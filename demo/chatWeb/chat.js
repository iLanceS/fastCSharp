'use strict';
var fastCSharp;
(function (fastCSharp) {
	var PollParameter = (function () {
		function PollParameter() {
		}
		return PollParameter;
	}());
	fastCSharp.PollParameter = PollParameter;
	var Poll = (function (_super) {
		fastCSharp.Pub.Extends(Poll, _super);
		function Poll(Parameter) {
			if (Parameter === void 0) { Parameter = null; }
			_super.call(this);
			fastCSharp.Pub.GetParameter(this, Poll.DefaultParameter, Parameter);
			fastCSharp.Pub.GetEvents(this, Poll.DefaultEvents, Parameter);
			if (!this.Query)
				this.Query = {};
			if (this.Path == null)
				this.Path = this.IsView ? '/poll.html' : 'poll';
			this.GetFunction = fastCSharp.Pub.ThisFunction(this, this.Get);
			this.VerifyFunction = fastCSharp.Pub.ThisFunction(this, this.Verify);
			this.OnVerifyFunction = fastCSharp.Pub.ThisFunction(this, this.OnVerify);
			fastCSharp.IndexPool.Push(this);
		}
		Poll.prototype.Start = function (Verify) {
			if (Verify != null)
				this.VerifyInfo = Verify;
			if (!this.Identity)
				setTimeout(this.GetFunction, this.Identity = 1);
		};
		Poll.prototype.Close = function () {
			fastCSharp.IndexPool.Pop(this);
		};
		Poll.prototype.Stop = function () {
			this.Identity = 0;
		};
		Poll.prototype.Get = function () {
			if (this.Identity) {
				if (this.OnMessage.Get().length) {
					fastCSharp.Pub.AppendJs((this.Domain == null ? '//127.0.0.1:8112' : (this.Domain ? '//' + this.Domain : '')) + '/ajax?n=' + this.Path + '&c=' + fastCSharp.IndexPool.ToString(this) + '.OnGet' + (this.IsView ? 'View' : '') + '&j=' + fastCSharp.Pub.ToJson({ verify: this.VerifyInfo, query: this.Query }, true, false).Escape() + (fastCSharp.Pub.IE ? '&t=' + (new Date).getTime() : ''), null, null, fastCSharp.Pub.ThisFunction(this, this.OnError, [this.Identity]));
				}
				else
					setTimeout(this.GetFunction, 1000);
			}
		};
		Poll.prototype.OnGet = function (Value) {
			this.OnGetView(Value ? Value.Return : null);
		};
		Poll.prototype.OnGetView = function (Value) {
			if (this.Identity) {
				++this.Identity;
				if (Value) {
					if (!this.VerifyPath || Value.isVerify) {
						this.VerifyTimeout = 100;
						if (this.ReturnName) {
							if (Value[this.ReturnName])
								this.OnMessage.Function(Value[this.ReturnName]);
						}
						else
							this.OnMessage.Function(Value);
						if (this.Identity)
							setTimeout(this.GetFunction, 100);
					}
					else {
						if ((this.VerifyTimeout *= 2) > 2000)
							this.VerifyTimeout = 2000;
						this.Verify();
					}
				}
				else
					setTimeout(this.GetFunction, 2000);
			}
		};
		Poll.prototype.OnError = function (Event, Identity) {
			if (Identity == this.Identity)
				setTimeout(fastCSharp.Pub.ThisFunction(this, this.Check, [Identity]), 2000);
		};
		Poll.prototype.Check = function (Identity) {
			if (Identity == this.Identity)
				setTimeout(this.GetFunction, 8000);
		};
		Poll.prototype.Verify = function () {
			fastCSharp.HttpRequest.Post(this.VerifyPath, null, this.OnVerifyFunction);
		};
		Poll.prototype.OnVerify = function (Value) {
			if (Value && Value.isVerify) {
				this.VerifyInfo = Value.Return;
				if (this.Identity)
					setTimeout(this.GetFunction, this.VerifyTimeout);
			}
			else if (this.Identity)
				setTimeout(this.VerifyFunction, 8000);
		};
		Poll.DefaultParameter = { Domain: null, Path: null, VerifyPath: 'poll.Verify', Query: null, IsView: true, ReturnName: null, VerifyTimeout: 100 };
		Poll.DefaultEvents = { OnMessage: null };
		Poll.Default = new Poll();
		return Poll;
	}(PollParameter));
	fastCSharp.Poll = Poll;
})(fastCSharp || (fastCSharp = {}));

var Chat = (function () {
	function Chat() {
	}
	Chat.Load = function () {
		var PollParameter = new fastCSharp.PollParameter(), PollEvents = PollParameter;
		PollParameter.VerifyPath = '';
		PollEvents.OnMessage = fastCSharp.Pub.ThisFunction(this, this.OnPoll);
		this.Poll = new fastCSharp.Poll(PollParameter);
		this.Login();
	};
	Chat.Login = function () {
		this.User = fastCSharp.HtmlElement.$Id('User').Value0().Trim();
		if (this.User) {
			fastCSharp.Skin.BodyData('LoginUser').$Set(this.User);
			fastCSharp.Skin.BodyData('IsLogining').$Set(1);
			fastCSharpAPI.ajax.user.Login(this.User, 0, fastCSharp.Pub.ThisFunction(this, this.OnLogin));
		}
	};
	Chat.OnLogin = function (Value) {
		fastCSharp.Skin.BodyData('IsLogining').$Set(0);
		if (Value.Return) {
			this.Poll.Query['UserVersion'] = Value.version;
			fastCSharp.Skin.BodyData('LoginUser').$Set(this.User);
			fastCSharp.Skin.BodyData('IsLogin').$Set(1);
			this.SetUsers(Value.Return);
			fastCSharp.Skin.Refresh();
			var HtmlEditor = fastCSharp.Declare.Getters['HtmlEditor']('MessageEditor', false);
			HtmlEditor.SetHtml('大家好，我是 ' + this.User);
			this.Send();
			this.Poll.Start(this.User);
		}
		else if (fastCSharp.HttpRequest.CheckError(Value)) {
			alert('用户已注册 ' + this.User);
			fastCSharp.Skin.BodyData('LoginUser').$Set('');
		}
	};
	Chat.SelectAllUser = function (IsSelect) {
		fastCSharp.Skin.BodyData('Users')
			.$For(function (Data) { Data.$('IsSelected').$Set(IsSelect); });
	};
	Chat.ChangeSelectAllUser = function () {
		fastCSharp.Skin.BodyData('Users')
			.$For(function (Data) { Data.$('IsSelected').$Not(); });
	};
	Chat.ChangeSelectUser = function (User) {
		fastCSharp.Skin.BodyData('Users')
			.$Find(function (Data) { return Data.User == User; })
			.For(function (Data) { Data.$('IsSelected').$Not(); });
	};
	Chat.SetUsers = function (Users) {
		var SkinUsers = fastCSharp.Skin.BodyData('Users');
		var SelectedUsers = SkinUsers.$Data ? SkinUsers.$Data.Find(function (User) { return User.IsSelected; }).ToHash(function (User) { return User.User; }) : {};
		SkinUsers.$Set(Users.RemoveAt(Users.indexOf(this.User)).ToArray(function (User) { return { User: User, IsSelected: !!SelectedUsers[User] }; }));
	};
	Chat.OnPoll = function (Value) {
		if (this.User && Value.Message) {
			if (Value.Message.Users) {
				this.Poll.Query['UserVersion'] = Value.Message.UserVersion;
				this.SetUsers(Value.Message.Users);
			}
			fastCSharp.Skin.BodyData('Messages').$Pushs(Value.Message.Messages);
		}
	};
	Chat.Logout = function () {
		fastCSharpAPI.ajax.user.Logout(this.User);
		this.Poll.Stop();
		this.User = null;
		var SkinData = fastCSharp.Skin.BodyData();
		SkinData.$('Users').$Set([]);
		SkinData.$('Messages').$Set([]);
		SkinData.$('LoginUser').$Set('');
		SkinData.$('IsLogin').$Set(0);
		SkinData.$('IsLogining').$Set(0);
	};
	Chat.Send = function () {
		var SelectedUsers = fastCSharp.Skin.BodyData('Users').$Find(function (User) { return User.IsSelected; });
		var HtmlEditor = fastCSharp.Declare.Getters['HtmlEditor']('MessageEditor', false), Message = HtmlEditor.GetHtml();
		fastCSharpAPI.ajax.user.Send(this.User, Message, (SelectedUsers.length ? SelectedUsers.ToArray(function (Data) { return Data.$Data.User; }) : null));
		fastCSharp.Skin.BodyData('Messages').$Push({ User: this.User, Time: new Date, Message: Message });
		HtmlEditor.SetHtml('');
	};
	return Chat;
}());
fastCSharp.Pub.OnModule(['htmlEditor'], fastCSharp.Pub.ThisFunction(Chat, Chat.Load), true);
