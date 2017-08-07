/// <reference path = "./base.page.ts" />
'use strict';
//鼠标覆盖图片修改	<img mouseimage="{OverSrc:'over.jpg',OutSrc:'out.jpg'}" id="XXX" />
var fastCSharp;
(function (fastCSharp) {
    var MouseImage = (function () {
        function MouseImage(Parameter) {
            fastCSharp.Pub.GetParameter(this, MouseImage.DefaultParameter, Parameter);
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        MouseImage.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (Element != this.Element) {
                    this.Element = Element;
                    fastCSharp.HtmlElement.$AddEvent(Element, ['mouseout'], fastCSharp.Pub.ThisEvent(this, this.Out));
                }
                Element.src = this.OverSrc;
            }
        };
        MouseImage.prototype.Out = function (Event) {
            fastCSharp.HtmlElement.$IdElement(this.Id).src = this.OutSrc;
        };
        MouseImage.DefaultParameter = { Id: null, Event: null, OverSrc: null, OutSrc: null };
        return MouseImage;
    }());
    fastCSharp.MouseImage = MouseImage;
    new fastCSharp.Declare(MouseImage, 'MouseImage', 'mouseover', 'AttributeName');
})(fastCSharp || (fastCSharp = {}));
