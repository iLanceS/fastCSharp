/// <reference path = "../base.page.ts" />
'use strict';
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
        __extends(Ace, _super);
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
