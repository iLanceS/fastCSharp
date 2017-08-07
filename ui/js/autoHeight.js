/// <reference path = "./base.page.ts" />
'use strict';
//TextArea高度自适应控件	<textarea autoheight="{LineHeight:20}" id="XXX"></textarea>
var fastCSharp;
(function (fastCSharp) {
    var AutoHeight = (function () {
        function AutoHeight(Parameter) {
            fastCSharp.Pub.GetParameter(this, AutoHeight.DefaultParameter, Parameter);
            var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
            if (this.LineHeight == null)
                this.LineHeight = parseInt(0 + fastCSharp.HtmlElement.$AttributeOrStyle(Element, 'lineHeight'));
            this.SetFunction = fastCSharp.Pub.ThisFunction(this, this.SetHeight);
            this.Div = fastCSharp.HtmlElement.$Create('div').AddClass(Element.className).Display(0).To();
            this.Start(this.Event || fastCSharp.DeclareEvent.Default);
        }
        AutoHeight.prototype.Start = function (Event) {
            if (!Event.IsGetOnly) {
                var Element = fastCSharp.HtmlElement.$IdElement(this.Id);
                if (!this.Element || Element != this.Element) {
                    this.MinHeight = (this.Element = Element).scrollHeight;
                    fastCSharp.HtmlElement.$AddEvent(Element, ['blur'], fastCSharp.Pub.ThisFunction(this, this.Stop));
                    fastCSharp.HtmlElement.$AddEvent(Element, ['keypress', 'keyup'], this.SetFunction);
                    var Width = fastCSharp.HtmlElement.$Width(Element), Css = fastCSharp.HtmlElement.$Css(Element);
                    if (!fastCSharp.Pub.IsBorder) {
                        Width -= parseInt(0 + Css['border-left-width'], 10) + parseInt(0 + Css['border-right-width'], 10);
                        if (!fastCSharp.Pub.IsPadding)
                            Width -= parseInt(0 + Css['padding-left'], 10) + parseInt(0 + Css['padding-right'], 10);
                    }
                    this.Div.Style('width', Width + 'px');
                    if (this.Interval)
                        clearTimeout(this.Interval);
                    this.IsStart = false;
                }
                if (!this.IsStart) {
                    if (!this.Interval)
                        this.Interval = setTimeout(this.SetFunction, this.Timeout);
                    this.IsStart = true;
                }
            }
        };
        AutoHeight.prototype.SetHeight = function () {
            if (this.IsStart) {
                this.Div.Html(this.Element.value.ToHTML().replace(/\r\n/g, '<br />').replace(/[\r\n]/g, '<br />'));
                var Height = this.Div.ScrollHeight0() + this.LineHeight;
                if (this.MaxHeight && Height > this.MaxHeight)
                    Height = this.MaxHeight;
                fastCSharp.HtmlElement.$SetStyle(this.Element, 'height', Math.max(Height, this.MinHeight) + 'px');
                if (this.Interval)
                    clearTimeout(this.Interval);
                this.Interval = setTimeout(this.SetFunction, this.Timeout);
            }
        };
        AutoHeight.prototype.Stop = function () {
            if (this.Interval) {
                clearTimeout(this.Interval);
                this.Interval = 0;
            }
            this.IsStart = false;
        };
        AutoHeight.DefaultParameter = { Id: null, Event: null, LineHeight: null, MaxHeight: 0, Timeout: 200 };
        return AutoHeight;
    }());
    fastCSharp.AutoHeight = AutoHeight;
    new fastCSharp.Declare(AutoHeight, 'AutoHeight', 'focus', 'Src');
})(fastCSharp || (fastCSharp = {}));
