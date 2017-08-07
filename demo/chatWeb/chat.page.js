/// <reference path = "../../ui/js/poll.ts" />
/// <reference path = "../../ui/js/htmlEditor.page.ts" />
/// <reference path = "./viewJs/api.ts" />
/*include:js\poll*/
'use strict';
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
        if (Value.__AJAXRETURN__) {
            this.Poll.Query['UserVersion'] = Value.version;
            fastCSharp.Skin.BodyData('LoginUser').$Set(this.User);
            fastCSharp.Skin.BodyData('IsLogin').$Set(1);
            this.SetUsers(Value.__AJAXRETURN__);
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
