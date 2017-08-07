
'use strict';
var fastCSharp;
(function (fastCSharp) {
	var AceParameter = (function () {
		function AceParameter() {
		}
		return AceParameter;
	}());
	var AceSessionIE6 = (function () {
		function AceSessionIE6(Editor) {
			this.Editor = Editor;
		}
		AceSessionIE6.prototype.getLength = function () {
			var Code = fastCSharp.HtmlElement.$Id(this.Editor.Id).Value0();
			return Code ? 0 : Code.length;
		};
		AceSessionIE6.prototype.getScreenLength = function () {
			return 0;
		};
		AceSessionIE6.prototype.setMode = function (Mode) { };
		AceSessionIE6.prototype.setTheme = function (Theme) { };
		AceSessionIE6.prototype.setUseWrapMode = function (IsWrap) { };
		AceSessionIE6.prototype.on = function (Name, Function) { };
		return AceSessionIE6;
	}());
	var AceEditorIE6 = (function () {
		function AceEditorIE6(Ace) {
			this.Ace = Ace;
			var Div = fastCSharp.HtmlElement.$Id(Ace.Id);
			if (Ace.Code == null)
				Ace.Code = Div.Text0();
			Div.Html('<textarea id="' + (this.Id = 'AceIe6_' + Ace.Id) + '" style="width:' + Div.Width0() + 'px;height:' + Div.Height0() + 'px"></textarea>');
			this.Session = new AceSessionIE6(this);
		}
		AceEditorIE6.prototype.getSession = function () {
			return this.Session;
		};
		AceEditorIE6.prototype.setValue = function (Code) {
			fastCSharp.HtmlElement.$SetValueById(this.Id, Code);
		};
		AceEditorIE6.prototype.getValue = function () {
			return fastCSharp.HtmlElement.$GetValueById(this.Id);
		};
		AceEditorIE6.prototype.setTheme = function (Theme) { };
		AceEditorIE6.prototype.setReadOnly = function (IsReadOnly) {
			fastCSharp.HtmlElement.$Id(this.Id).Set('readOnly', IsReadOnly);
		};
		AceEditorIE6.prototype.moveCursorTo = function (Row, Col) { };
		AceEditorIE6.prototype.focus = function () {
			fastCSharp.HtmlElement.$Id(this.Id).Focus0();
		};
		AceEditorIE6.prototype.resize = function () { };
		AceEditorIE6.prototype.on = function (Name, Function) { };
		return AceEditorIE6;
	}());
	var Ace = (function (_super) {
		fastCSharp.Pub.Extends(Ace, _super);
		function Ace(Parameter) {
			_super.call(this);
			fastCSharp.Pub.GetParameter(this, Ace.DefaultParameter, Parameter);
			(this.OnChange = new fastCSharp.Events).Add(fastCSharp.Pub.ThisFunction(this, this.Resize));
		}
		Ace.prototype.Check = function () {
			if (fastCSharp.HtmlElement.$Id(this.Id).Attribute0('ace') == 'ace')
				return this;
			var Value = new Ace(this.Parameter);
			Value.Show();
			return Value;
		};
		Ace.prototype.Show = function () {
			fastCSharp.Skin.Refresh();
			var Div = fastCSharp.HtmlElement.$Id(this.Id);
			if (Div.Element0()) {
				if (Ace.IsIE6)
					this.Editor = new AceEditorIE6(this);
				else {
					var Height = this.IsReadOnly ? 0 : Div.Height0();
					(this.Editor = window['ace'].edit(this.Id)).setFontSize(this.FontSize);
					var LineHeight = this.Editor.renderer.lineHeight || 14;
					if (this.IsReadOnly) {
						this.MinLength = 1;
						Div.Style('height', (((this.LastLength = this.Editor.getSession().getLength()) + (this.IsWrap ? 0 : 1)) * LineHeight + 2) + 'px');
					}
					else {
						if (!this.MinLength)
							this.MinLength = Math.floor((Height + LineHeight - 1) / LineHeight);
						Div.Style('height', (((this.LastLength = this.MinLength) + (this.IsWrap ? 0 : 1)) * LineHeight + 2) + 'px');
					}
					var Session = this.Editor.getSession();
					Session.setMode('ace/mode/' + this.Mode);
					Session.setUseWrapMode(this.IsWrap);
					Session.on('change', this.OnChange.Function);
					Session.on('changeFold', this.OnChange.Function);
					this.Editor.setTheme('ace/theme/' + this.Theme);
				}
				if (this.Code != null)
					this.Editor.setValue(this.Code);
				this.Editor.setReadOnly(this.IsReadOnly);
				this.Editor.moveCursorTo(0, 0);
				if (!this.IsReadOnly)
					this.Editor.focus();
				Div.Set('ace', 'ace');
			}
		};
		Ace.prototype.Set = function (Value) {
			var Session = this.Editor.getSession();
			if (Value.Mode)
				Session.setMode('ace/mode/' + (this.Mode = Value.Mode));
			if (Value.Theme)
				Session.setTheme('ace/theme/' + (this.Theme = Value.Theme));
			if (Value.Code != null)
				this.Editor.setValue(Value.Code);
			this.Editor.moveCursorTo(0, 0);
			if (!this.IsReadOnly)
				this.Editor.focus();
		};
		Ace.prototype.Resize = function () {
			if (!Ace.IsIE6) {
				var Length = this.Editor.getSession().getScreenLength();
				if (this.MaxHeight) {
					var MaxLength = Math.floor(((this.MaxHeight < 0 ? (fastCSharp.HtmlElement.$Height() + this.MaxHeight) : this.MaxHeight) - 2) / this.Editor.renderer.lineHeight) - (this.IsWrap ? 0 : 1);
					if (Length > MaxLength)
						Length = MaxLength;
				}
				if (Length < this.MinLength)
					Length = this.MinLength;
				if (Length != this.LastLength) {
					fastCSharp.HtmlElement.$Id(this.Id).Style('height', (((this.LastLength = Length) + (this.IsWrap ? 0 : 1)) * this.Editor.renderer.lineHeight + 2) + 'px');
					this.Editor.resize();
				}
			}
		};
		Ace.LoadIE6 = function () {
			this.Load();
			this.Show();
		};
		Ace.Load = function () {
			if (fastCSharp.Pub.PageView.OnSet)
				fastCSharp.Pub.PageView.OnSet.Add(fastCSharp.Pub.ThisFunction(this, this.Show));
		};
		Ace.Show = function () {
			if (this.IsIE6 || window['ace']) {
				for (var Elements = fastCSharp.HtmlElement.$Name('ace').GetElements(), Index = 0; Index - Elements.length; ++Index) {
					if (!fastCSharp.Pub.GetHtmlEditor(Elements[Index])) {
						var Div = Elements[Index];
						if (Div.offsetHeight) {
							var Mode = fastCSharp.HtmlElement.$Attribute(Div, 'mode');
							if (!Div.id && Mode) {
								var ParameterString = fastCSharp.HtmlElement.$Attribute(Div, 'ace'), Parameter = ParameterString && ParameterString != 'ace' ? eval('(' + ParameterString + ')') : new AceParameter();
								Parameter.Id = Div.id = 'fastCSharpAce' + (++this.Identity);
								for (var CodeNodes = Div.childNodes, CodeIndex = 0; CodeIndex !== CodeNodes.length; ++CodeIndex) {
									var Node = CodeNodes[CodeIndex];
									if (Node.tagName) {
										Parameter.Code = fastCSharp.HtmlElement.$GetText(Node).replace(/\xA0/g, ' ');
										break;
									}
								}
								Parameter.Mode = Mode;
								if (Parameter.IsReadOnly == null)
									Parameter.IsReadOnly = true;
								new Ace(Parameter).Show();
							}
						}
					}
				}
			}
		};
		Ace.LoadMoule = function (Function, IsLoad) {
			if (IsLoad === void 0) { IsLoad = true; }
			if (Function) {
				if (this.IsIE6) {
					if (IsLoad)
						fastCSharp.Pub.OnLoad(Function, null, true);
					else
						Function();
				}
				else
					fastCSharp.Pub.OnModule(['ace/ace'], Function, IsLoad);
			}
		};
		Ace.CheckIE6 = function () {
			if (fastCSharp.Pub.IE) {
				var Version = navigator.appVersion.match(/MSIE\s+(\d+)/);
				if (Version && Version.length == 2 && parseInt('0' + Version[1], 10) < 7) {
					fastCSharp.Pub.OnLoad(this.LoadIE6, this, this.IsIE6 = true);
					return;
				}
			}
			fastCSharp.Pub.OnLoad(this.Load, this, true);
			fastCSharp.Pub.OnModule(['ace/ace'], fastCSharp.Pub.ThisFunction(this, this.Show), true);
		};
		Ace.DefaultParameter = { Id: null, MinLength: null, MaxHeight: 0, FontSize: 12, Code: '', Mode: 'csharp', Theme: 'eclipse', IsWrap: true, IsReadOnly: false };
		Ace.Identity = 0;
		return Ace;
	}(AceParameter));
	fastCSharp.Ace = Ace;
	fastCSharp.Ace.CheckIE6();
})(fastCSharp || (fastCSharp = {}));
var fastCSharp;
(function (fastCSharp) {
	var Menu = (function () {
		function Menu(Parameter) {
			fastCSharp.Pub.GetParameter(this, Menu.NenuParameter, Parameter);
			fastCSharp.Pub.GetEvents(this, Menu.MenuEvents, Parameter);
		}
		Menu.prototype.ShowMenu = function () {
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId), Skin;
			if (this.IsMove)
				Menu.Style('position', 'absolute').Style('zIndex', fastCSharp.HtmlElement.ZIndex + this.ZIndex);
			if (this.SkinView)
				Skin = fastCSharp.Skin.Skins[this.MenuId];
			var IsShow = this.ShowViewFunctionName == null || this.SkinView[this.ShowViewFunctionName]();
			if (Skin && this.ShowView != this.SkinView) {
				this.ShowView = this.SkinView;
				if (IsShow)
					Skin.Show(this.SkinView);
			}
			if (this.IsShow && IsShow)
				Menu.Display(1);
			var Element = fastCSharp.HtmlElement.$(this.Element);
			if (this.OutClassName)
				Element.RemoveClass(this.OutClassName);
			if (this.OverClassName)
				Element.AddClass(this.OverClassName);
		};
		Menu.prototype.HideMenu = function () {
			var Element = fastCSharp.HtmlElement.$(this.Element);
			if (this.OverClassName)
				Element.RemoveClass(this.OverClassName);
			if (this.OutClassName)
				Element.AddClass(this.OutClassName);
			if (this.IsShow)
				fastCSharp.HtmlElement.$Id(this.MenuId).Display(0);
			this.ShowView = null;
		};
		Menu.prototype.CheckScroll = function (Left, Top, XY) {
			if (XY === void 0) { XY = null; }
			if (this.Left)
				Left += this.Left;
			if (this.Top)
				Top += this.Top;
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId);
			if (this.IsCheckScroll) {
				var ScrollLeft = fastCSharp.HtmlElement.$GetScrollLeft(), ScrollTop = fastCSharp.HtmlElement.$GetScrollTop();
				if (XY) {
					var Width = fastCSharp.HtmlElement.$Width(Menu.Element0()), Height = fastCSharp.HtmlElement.$Height(Menu.Element0());
					if (Width) {
						var ClientWidth = fastCSharp.HtmlElement.$Width() + ScrollLeft;
						if (ClientWidth > XY.Left && Left > (ClientWidth -= Width))
							Left = ClientWidth;
					}
					if (Height) {
						var ClientHeight = fastCSharp.HtmlElement.$Height() + ScrollTop;
						if (ClientHeight > XY.Top && Top > (ClientHeight -= Height))
							Top = ClientHeight;
					}
				}
				if (Left < ScrollLeft)
					Left = ScrollLeft;
				if (Top < ScrollTop)
					Top = ScrollTop;
			}
			Menu.ToXY(Left, Top);
		};
		Menu.prototype.ToTopLeft = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left, XY.Top - fastCSharp.HtmlElement.$Height(fastCSharp.HtmlElement.$IdElement(this.MenuId)), XY);
		};
		Menu.prototype.ToTopRight = function (Event, Element) {
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId), XY = Element.XY0();
			this.CheckScroll(XY.Left + fastCSharp.HtmlElement.$Width(Element.Element0()) - fastCSharp.HtmlElement.$Width(Menu.Element0()), XY.Top - fastCSharp.HtmlElement.$Height(Menu.Element0()), XY);
		};
		Menu.prototype.ToTop = function (Event, Element) {
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId), XY = Element.XY0();
			this.CheckScroll(XY.Left + (fastCSharp.HtmlElement.$Width(Element.Element0()) - fastCSharp.HtmlElement.$Width(Menu.Element0())) / 2, XY.Top - fastCSharp.HtmlElement.$Height(Menu.Element0()), XY);
		};
		Menu.prototype.ToBottomLeft = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left, XY.Top + fastCSharp.HtmlElement.$Height(Element.Element0()), XY);
		};
		Menu.prototype.ToBottomRight = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left + fastCSharp.HtmlElement.$Width(Element.Element0()) - fastCSharp.HtmlElement.$Width(fastCSharp.HtmlElement.$IdElement(this.MenuId)), XY.Top + fastCSharp.HtmlElement.$Height(Element.Element0()), XY);
		};
		Menu.prototype.ToBottom = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left + (fastCSharp.HtmlElement.$Width(Element.Element0()) - fastCSharp.HtmlElement.$Width(fastCSharp.HtmlElement.$IdElement(this.MenuId))) / 2, XY.Top + fastCSharp.HtmlElement.$Height(Element.Element0()), XY);
		};
		Menu.prototype.ToLeftTop = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left - fastCSharp.HtmlElement.$Width(fastCSharp.HtmlElement.$IdElement(this.MenuId)), XY.Top, XY);
		};
		Menu.prototype.ToLeftBottom = function (Event, Element) {
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId), XY = Element.XY0();
			this.CheckScroll(XY.Left - fastCSharp.HtmlElement.$Width(Menu.Element0()), XY.Top + fastCSharp.HtmlElement.$Height(Element.Element0()) - fastCSharp.HtmlElement.$Height(Menu.Element0()), XY);
		};
		Menu.prototype.ToLeft = function (Event, Element) {
			var Menu = fastCSharp.HtmlElement.$Id(this.MenuId), XY = Element.XY0();
			this.CheckScroll(XY.Left - fastCSharp.HtmlElement.$Width(Menu.Element0()), XY.Top + (fastCSharp.HtmlElement.$Height(Element.Element0()) - fastCSharp.HtmlElement.$Height(Menu.Element0())) / 2, XY);
		};
		Menu.prototype.ToRightTop = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left + fastCSharp.HtmlElement.$Width(Element.Element0()), XY.Top, XY);
		};
		Menu.prototype.ToRightBottom = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left + fastCSharp.HtmlElement.$Width(Element.Element0()), XY.Top + fastCSharp.HtmlElement.$Height(Element.Element0()) - fastCSharp.HtmlElement.$Height(fastCSharp.HtmlElement.$IdElement(this.MenuId)), XY);
		};
		Menu.prototype.ToRight = function (Event, Element) {
			var XY = Element.XY0();
			this.CheckScroll(XY.Left + fastCSharp.HtmlElement.$Width(Element.Element0()), XY.Top + (fastCSharp.HtmlElement.$Height(Element.Element0()) - fastCSharp.HtmlElement.$Height(fastCSharp.HtmlElement.$IdElement(this.MenuId))) / 2, XY);
		};
		Menu.NenuParameter = { Id: null, Event: null, MenuId: null, SkinView: null, ShowViewFunctionName: null, Type: null, Top: null, Left: null, OverClassName: null, OutClassName: null, IsCheckScroll: 1, IsShow: 1, IsMove: 1, ZIndex: 1 };
		Menu.MenuEvents = { OnStart: null, OnShowed: null };
		return Menu;
	}());
	fastCSharp.Menu = Menu;
})(fastCSharp || (fastCSharp = {}));

var fastCSharp;
(function (fastCSharp) {
	var MouseMenu = (function (_super) {
		fastCSharp.Pub.Extends(MouseMenu, _super);
		function MouseMenu(Parameter) {
			_super.call(this, Parameter);
			fastCSharp.Pub.GetParameter(this, MouseMenu.DefaultParameter, Parameter);
			fastCSharp.Pub.GetEvents(this, MouseMenu.DefaultEvents, Parameter);
			this.HideFunction = fastCSharp.Pub.ThisFunction(this, this.Hide);
			this.MouseOutFunction = fastCSharp.Pub.ThisFunction(this, this.MouseOut);
			this.Start(this.Event || fastCSharp.DeclareEvent.Default);
		}
		MouseMenu.prototype.Start = function (Event) {
			if (!Event.IsGetOnly) {
				this.OnStart.Function(this);
				var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
				if (Element != this.Element) {
					this.Element = Element;
					fastCSharp.HtmlElement.$AddEvent(Element, ['mouseout'], this.MouseOutFunction);
					if (this.IsMouseMove)
						fastCSharp.HtmlElement.$AddEvent(Element, ['mousemove'], fastCSharp.Pub.ThisEvent(this, this.ReShow));
					this.CheckMenuParameter(true);
				}
				this.ClearInterval();
				this.IsOver = true;
				this.Show(Event);
			}
		};
		MouseMenu.prototype.CheckMenuParameter = function (IsStart) {
			if (IsStart === void 0) { IsStart = false; }
			var Element = fastCSharp.HtmlElement.$Id(this.MenuId);
			if (Element.Element0()) {
				var Parameter = fastCSharp.HtmlElement.$Attribute(Element.Element0(), 'mousemenu');
				if (Parameter != null) {
					var Id = eval('(' + Parameter + ')').Id;
					if (Id != this.Id) {
						Parameter = null;
						var Menu = fastCSharp.Declare.Getters['MouseMenu'](Id, true);
						if (Menu) {
							Menu.Remove();
							Element.Set('mousemenu', '{Id:"' + this.Id + '"}');
							return;
						}
					}
				}
				if (IsStart) {
					if (Parameter == null)
						Element.Set('mousemenu', '{Id:"' + this.Id + '"}');
					Element.AddEvent('mouseout', this.MouseOutFunction);
				}
			}
		};
		MouseMenu.prototype.Show = function (Event) {
			this.ShowMenu();
			if (this.IsMove)
				this.ReShow(Event, fastCSharp.HtmlElement.$Id(this.Id));
			this.OnShowed.Function();
		};
		MouseMenu.prototype.MouseOut = function () {
			this.IsOver = false;
			this.ClearInterval();
			this.HideInterval = setTimeout(this.HideFunction, this.Timeout);
		};
		MouseMenu.prototype.ClearInterval = function () {
			if (this.HideInterval) {
				clearTimeout(this.HideInterval);
				this.HideInterval = 0;
			}
		};
		MouseMenu.prototype.Hide = function () {
			this.ClearInterval();
			this.HideMenu();
		};
		MouseMenu.prototype.Remove = function () {
			this.ClearInterval();
			this.ShowView = null;
			var Element = fastCSharp.HtmlElement.$Id(this.Id);
			Element.DeleteEvent('mouseout', this.MouseOutFunction);
			fastCSharp.HtmlElement.$Id(this.MenuId).Set('mousemenu', '').DeleteEvent('mouseout', this.MouseOutFunction);
			this.Element = null;
		};
		MouseMenu.prototype.ReShow = function (Event, Element) {
			this.OnMove.Function(Event, this);
			this['To' + (this.Type || 'Mouse')](Event, Element);
		};
		MouseMenu.prototype.ToMouse = function (Event, Element) {
			this.CheckScroll(Event.clientX, Event.clientY);
		};
		MouseMenu.DefaultParameter = { Timeout: 100, IsMouseMove: 0 };
		MouseMenu.DefaultEvents = { OnMove: null };
		return MouseMenu;
	}(fastCSharp.Menu));
	fastCSharp.MouseMenu = MouseMenu;
	var MouseMenuEnum = (function () {
		function MouseMenuEnum(Value, Show) {
			this.Value = Value;
			this.Show = Show || Value;
		}
		MouseMenuEnum.prototype.ToJson = function (IsIgnore, IsNameQuery, Parents) {
			return fastCSharp.Pub.ToJson(this.Value, IsIgnore, IsNameQuery, Parents);
		};
		MouseMenuEnum.prototype.toString = function () {
			return this.Value;
		};
		return MouseMenuEnum;
	}());
	fastCSharp.MouseMenuEnum = MouseMenuEnum;
	new fastCSharp.Declare(MouseMenu, 'MouseMenu', 'mouseover', 'ParameterId');
})(fastCSharp || (fastCSharp = {}));

var CodeMenu = (function () {
	function CodeMenu() {
	}
	CodeMenu.Show = function (Menu) {
		Menu.MenuId = 'codeMenu';
		Menu.Type = Menu.Type || 'Top';
		if (CodeMenu.CurrentMenu != Menu)
			CodeMenu.Check(Menu);
	};
	CodeMenu.Check = function (Menu) {
		var SkinView = (this.CurrentMenu = Menu).SkinView, Item = this.Items[SkinView.File];
		Menu.CheckMenuParameter();
		if (Item) {
			SkinView.Item = Item;
			Menu.IsShow = Item != this.NullItem;
		}
		else {
			Menu.IsShow = false;
			fastCSharpAPI.include.codeMenu(SkinView.File, fastCSharp.Pub.ThisFunction(this, this.OnGetMenu, [Menu]));
		}
		fastCSharp.HtmlElement.$Id('codeMenu').Display(Menu.IsShow);
	};
	CodeMenu.OnGetMenu = function (Value, Menu) {
		if (Value && !Value.ErrorRequest) {
			this.Items[Menu.SkinView.File] = Value.Item || this.NullItem;
			if (Value.Item && Menu == this.CurrentMenu && Menu.IsOver) {
				Menu.SkinView.Item = Value.Item;
				Menu.IsShow = true;
				fastCSharp.Skin.Skins['codeMenu'].Show(Menu.SkinView);
				if (Menu.Type != 'Mouse')
					Menu['To' + Menu.Type](null, fastCSharp.HtmlElement.$Id(Menu.Id));
			}
		}
	};
	CodeMenu.OpenFile = function () {
		fastCSharpAPI.ajax.pub.OpenFile(this.CurrentMenu.SkinView.Item.File.FullName);
		this.CurrentMenu.Hide();
	};
	CodeMenu.Items = {};
	CodeMenu.NullItem = {};
	return CodeMenu;
}());

 
var HideButton = (function () {
	function HideButton() {
	}
	HideButton.ChangeNoCookie = function (Name) {
		fastCSharp.Skin.BodyData('IsHide' + Name).$Not();
	};
	HideButton.SetCookie = function (Name, IsHide) {
		var Value = fastCSharp.Cookie.Default.Read('HideButton');
		if (IsHide)
			Value = Value ? Value.split(',').RemoveValue(Name).Push(Name).join(',') : Name;
		else
			Value = Value ? Value.split(',').RemoveValue(Name).join(',') : '';
		fastCSharp.Cookie.Default.Write({ Name: 'HideButton', Value: Value });
	};
	HideButton.Change = function (Name) {
		var IsHide = fastCSharp.Skin.BodyData('IsHide' + Name).$Not();
		if (IsHide)
			this.SetCookie(Name, IsHide);
		else
			Mark.To(Name, true, false);
	};
	HideButton.TryShow = function (Name, IsShow, IsCookie) {
		if (IsShow === void 0) { IsShow = true; }
		if (IsCookie === void 0) { IsCookie = true; }
		if (fastCSharp.Skin.Body.Data['IsHide' + Name] ? IsShow : !IsShow) {
			fastCSharp.Skin.BodyData('IsHide' + Name).$Set(!IsShow);
			if (IsCookie)
				this.SetCookie(Name, !IsShow);
		}
	};
	HideButton.OnLoad = function (Function) {
		if (this.LoadEvents)
			this.LoadEvents.Add(Function);
		else
			Function();
	};
	HideButton.Load = function () {
		var Name = fastCSharp.Cookie.Default.Read('HideButton');
		if (Name) {
			for (var Names = Name.split(','), Index = Names.length; Index; this.TryShow(Names[--Index], false, false))
				;
		}
		this.LoadEvents.Function();
		this.LoadEvents = null;
	};
	HideButton.LoadEvents = new fastCSharp.Events();
	return HideButton;
}());
fastCSharp.Pub.OnLoad(HideButton.Load, HideButton, true);

var Mark = (function () {
	function Mark(Id) {
		var Ids = (this.Id = Id).split('.');
		this.Title = fastCSharp.HtmlElement.$GetText(fastCSharp.HtmlElement.$IdElement(this.HideButtonName = Ids[0]));
		if (Ids.length !== 1) {
			var Element = fastCSharp.HtmlElement.$IdElement(Id);
			if (Element)
				this.Title += ' - ' + fastCSharp.HtmlElement.$GetText(Element);
			else {
				HideButton.TryShow(this.HideButtonName, true, false);
				fastCSharp.Skin.Refresh();
				this.Title += ' - ' + fastCSharp.HtmlElement.$GetText(fastCSharp.HtmlElement.$IdElement(Id));
				HideButton.TryShow(this.HideButtonName, false, false);
			}
		}
		Mark.Ids[Id] = this;
	}
	Mark.To = function (Id, IsCookie, IsShowHideButton) {
		if (IsCookie === void 0) { IsCookie = true; }
		if (IsShowHideButton === void 0) { IsShowHideButton = true; }
		var Marks = fastCSharp.Skin.BodyData('Client')['Marks'], Item = Mark.Ids[Id] || new Mark(Id);
		if (IsShowHideButton)
			HideButton.TryShow(Item.HideButtonName);
		var Data = Marks.$Data.Remove(function (Data) { return Data.Id == Id; });
		if (Data.length == 8)
			Data.splice(0, 1);
		Data.push(Item);
		Marks.$Set(Data);
		this.ShowMark(Id);
		if (IsCookie)
			fastCSharp.Cookie.Default.Write({ Name: 'Mark', Value: Id });
	};
	Mark.ShowMark = function (Id) {
		fastCSharp.HtmlElement.$ScrollTopById(Id);
		if (Id != this.MarkId) {
			if (this.MarkId) {
				if (this.MarkInterval)
					clearInterval(this.MarkInterval);
				fastCSharp.HtmlElement.$Id(this.MarkId).Style('color', '').Style('font-size', '');
			}
			this.MarkId = Id;
			this.MarkLoopCount = 6;
			this.SetColor(true);
			this.MarkInterval = setInterval(fastCSharp.Pub.ThisFunction(this, this.SetColor), 200);
		}
	};
	Mark.SetColor = function (IsColor) {
		fastCSharp.HtmlElement.$Id(this.MarkId).Style('color', (this.MarkLoopCount & 1) ? '' : 'red').Style('font-size', (this.MarkLoopCount & 1) ? '' : '20px');
		if (--this.MarkLoopCount == 0) {
			clearInterval(this.MarkInterval);
			this.MarkId = this.MarkInterval = null;
		}
	};
	Mark.Load = function (IsHideButton) {
		if (IsHideButton) {
			var Client = fastCSharp.Skin.BodyData('Client');
			Client.$Data['Marks'] = [];
			Client.$ReShow();
			var MarkId = fastCSharp.Cookie.Default.Read('Mark', null);
			if (MarkId) {
				fastCSharp.Skin.Refresh();
				this.To(MarkId, false);
			}
		}
		else
			HideButton.OnLoad(fastCSharp.Pub.ThisFunction(this, this.Load, [true]));
	};
	Mark.Ids = {};
	return Mark;
}());
fastCSharp.Pub.OnLoad(Mark.Load, Mark, true);


