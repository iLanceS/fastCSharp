/// <reference path = "./base.page.ts" />
'use strict';
//输入框输入长度控件	<input inputlength="{Length:500,SkinId:'YYY',ErrorSkinId:'ZZZ'}" id="XXX" /><div id="YYY" SKIN="true" style="display:none">还可以输入=@InputLength字</div><div id="ZZZ" SKIN="true" style="display:none">已经超出=@InputLength字</div>
var fastCSharp;
(function (fastCSharp) {
    var InputLength = (function () {
        function InputLength(Parameter) {
            fastCSharp.Pub.GetParameter(this, InputLength.DefaultParameter, Parameter);
            if (!this.SkinView)
                this.SkinView = {};
            if (!this.ErrorSkinView)
                this.ErrorSkinView = {};
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        InputLength.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (Element != this.Element) {
                    this.Element = Element;
                    fastCSharp.HtmlElement.$AddEvent(Element, ['keypress', 'keyup', 'blur'], fastCSharp.Pub.ThisEvent(this, this.Check));
                }
            }
        };
        InputLength.prototype.Check = function () {
            var Length = fastCSharp.HtmlElement.$IdElement(this.Id).value.length;
            if (Length <= this.Length) {
                if (this.ErrorSkinId)
                    fastCSharp.Skin.Skins[this.ErrorSkinId].Hide();
                if (this.SkinId) {
                    this.SkinView[this.LengthName] = this.Length - Length;
                    fastCSharp.Skin.Skins[this.SkinId].Show(this.SkinView);
                }
            }
            else {
                if (this.SkinId)
                    fastCSharp.Skin.Skins[this.SkinId].Hide();
                if (this.ErrorSkinId) {
                    this.ErrorSkinView[this.ErrorLengthName] = Length - this.Length;
                    fastCSharp.Skin.Skins[this.ErrorSkinId].Show(this.ErrorSkinView);
                }
            }
        };
        InputLength.prototype.GetValue = function () {
            var Value = fastCSharp.HtmlElement.$IdElement(this.Id).value;
            return Value.length <= this.Length ? Value : null;
        };
        InputLength.DefaultParameter = { Id: null, Event: null, Length: 0, SkinId: null, LengthName: 'InputLength', SkinView: null, ErrorSkinId: null, ErrorLengthName: 'InputLength', ErrorSkinView: null };
        return InputLength;
    }());
    fastCSharp.InputLength = InputLength;
    new fastCSharp.Declare(InputLength, 'InputLength', 'focus', 'Src');
})(fastCSharp || (fastCSharp = {}));
