/// <reference path = "./base.page.ts" />
'use strict';
//键盘事件	<input inputkeycode="{Keys:{13:fastCSharp.Pub.Alert}}" id="XXX" />
var fastCSharp;
(function (fastCSharp) {
    var InputKeyCode = (function () {
        function InputKeyCode(Parameter) {
            fastCSharp.Pub.GetParameter(this, InputKeyCode.DefaultParameter, Parameter);
            fastCSharp.Pub.GetEvents(this, InputKeyCode.DefaultEvents, Parameter);
            if (!this.Keys)
                this.Keys = {};
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        InputKeyCode.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (Element != this.Element) {
                    this.Element = Element;
                    fastCSharp.HtmlElement.$AddEvent(Element, ['keyup', 'keypress'], fastCSharp.Pub.ThisEvent(this, this.OnKey));
                }
            }
        };
        InputKeyCode.prototype.OnKey = function (Event) {
            var Value = this.Keys[Event.keyCode];
            if (Value)
                Value();
            this.OnAnyKey.Function();
        };
        InputKeyCode.DefaultParameter = { Id: null, Event: null, Keys: null };
        InputKeyCode.DefaultEvents = { OnAnyKey: null };
        return InputKeyCode;
    }());
    fastCSharp.InputKeyCode = InputKeyCode;
    new fastCSharp.Declare(InputKeyCode, 'InputKeyCode', 'focus', 'Src');
})(fastCSharp || (fastCSharp = {}));
