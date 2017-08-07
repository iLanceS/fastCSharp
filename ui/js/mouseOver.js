/// <reference path = "./base.page.ts" />
'use strict';
//鼠标覆盖处理	<div mouseover="{}" id="XXX"></div>
var fastCSharp;
(function (fastCSharp) {
    var MouseOver = (function () {
        function MouseOver(Parameter) {
            fastCSharp.Pub.GetParameter(this, MouseOver.DefaultParameter, Parameter);
            fastCSharp.Pub.GetEvents(this, MouseOver.DefaultEvents, Parameter);
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        MouseOver.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (Element != this.Element) {
                    this.Element = Element;
                    fastCSharp.HtmlElement.$AddEvent(Element, ['mouseout'], fastCSharp.Pub.ThisEvent(this, this.Out));
                }
                this.OnOver.Function(Event, Element);
            }
        };
        MouseOver.prototype.Out = function (Event) {
            this.OnOut.Function(Event, fastCSharp.HtmlElement.$IdElement(this.Id));
        };
        MouseOver.DefaultParameter = { Id: null, Event: null };
        MouseOver.DefaultEvents = { OnOver: null, OnOut: null };
        return MouseOver;
    }());
    fastCSharp.MouseOver = MouseOver;
    new fastCSharp.Declare(MouseOver, 'MouseOver', 'mouseover', 'AttributeName');
})(fastCSharp || (fastCSharp = {}));
