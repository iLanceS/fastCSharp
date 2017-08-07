/// <reference path = "./base.page.ts" />
'use strict';
//鼠标拖拽	<div body="true" floatcenter="true" id="YYY" style="display:none"><div mousemove="{MoveId:'YYY'}" id="XXX"></div></div>
var fastCSharp;
(function (fastCSharp) {
    var MouseMove = (function () {
        function MouseMove(Parameter) {
            fastCSharp.Pub.GetParameter(this, MouseMove.DefaultParameter, Parameter);
            this.MoveEvent = fastCSharp.Pub.ThisEvent(this, this.Move);
            this.StopEvent = fastCSharp.Pub.ThisFunction(this, this.Stop);
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        MouseMove.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                if (this.IsStart)
                    this.Stop();
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (Element != this.Element) {
                    this.Element = Element;
                    fastCSharp.HtmlElement.$(document.body).AddEvent('mousedown,mouseup,blur', this.StopEvent).AddEvent('mousemove', this.MoveEvent);
                }
                var Element = fastCSharp.HtmlElement.$IdElement(this.MoveId);
                Element.style.position = 'absolute';
                this.Left = Element.offsetLeft;
                this.Top = Element.offsetTop;
                this.ClientX = Event.clientX;
                this.ClientY = Event.clientY;
                this.IsStart = true;
            }
        };
        MouseMove.prototype.Move = function (Event) {
            fastCSharp.HtmlElement.$Id(this.MoveId).Left(this.Left + Event.clientX - this.ClientX).Top(this.Top + Event.clientY - this.ClientY);
        };
        MouseMove.prototype.Stop = function () {
            if (this.IsStart) {
                fastCSharp.HtmlElement.$(document.body).DeleteEvent('mousedown,mouseup,blur', this.StopEvent).DeleteEvent('mousemove', this.MoveEvent);
                this.IsStart = false;
            }
        };
        MouseMove.DefaultParameter = { Id: null, Event: null, MoveId: null };
        return MouseMove;
    }());
    fastCSharp.MouseMove = MouseMove;
    new fastCSharp.Declare(MouseMove, 'MouseMove', 'mousedown', 'AttributeName');
})(fastCSharp || (fastCSharp = {}));
