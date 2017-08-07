/// <reference path = "./overDiv.ts" />
/*include:js\overDiv*/
//界面居中弹出层	<div body="true" floatcenter="{}" id="XXX" style="display:none"></div>
//fastCSharp.FloatCenter.FloatCenters.XXX.Show();
var fastCSharp;
(function (fastCSharp) {
    var FloatCenter = (function () {
        function FloatCenter(Parameter) {
            fastCSharp.Pub.GetParameter(this, FloatCenter.DefaultParameter, Parameter);
            this.KeyDownFunction = fastCSharp.Pub.ThisEvent(this, this.KeyDown);
            if (this.SkinView)
                this.Skin = fastCSharp.Skin.Skins[this.Id] || new fastCSharp.Skin(this.Id);
            if (fastCSharp.Pub.IsFixed) {
                if (!this.AutoClass)
                    fastCSharp.HtmlElement.$Id(this.Id).Style('position', 'fixed').Style('top,left', '50%').Style('transform', 'translateX(-50%) translateY(-50%)');
            }
            else {
                fastCSharp.HtmlElement.$Id(this.Id).Style('position', 'absolute');
                this.ShowFunction = fastCSharp.Pub.ThisFunction(this, this.Show);
                this.Hide();
            }
        }
        FloatCenter.prototype.Show = function () {
            var Element = fastCSharp.HtmlElement.$Id(this.Id);
            if (fastCSharp.Pub.IsFixed) {
                fastCSharp.OverDiv.Default.Show(this.Id, this.ZIndex || !this.AutoClass ? fastCSharp.HtmlElement.ZIndex + this.ZIndex : 0);
                Element.Style('zIndex', fastCSharp.HtmlElement.ZIndex + this.ZIndex);
                if (this.AutoClass) {
                    if (this.Skin)
                        this.Skin.SetHtml(this.SkinView);
                    Element.AddClass(this.AutoClass);
                }
                else {
                    if (this.Skin)
                        this.Skin.Show(this.SkinView);
                    else
                        Element.Display(1);
                }
            }
            else {
                fastCSharp.OverDiv.Default.Show();
                if (this.Skin)
                    this.Skin.Show(this.SkinView);
                else
                    Element.Display(1);
                var Left = (fastCSharp.HtmlElement.$GetScrollLeft() + (fastCSharp.HtmlElement.$Width() - fastCSharp.HtmlElement.$Width(Element.Element0())) / 2), Top = (fastCSharp.HtmlElement.$GetScrollTop() + (fastCSharp.HtmlElement.$Height() - fastCSharp.HtmlElement.$Height(Element.Element0())) / 2);
                Element.Style('zIndex', fastCSharp.HtmlElement.ZIndex + this.ZIndex).ToXY(Left < 0 ? 0 : Left, Top < 0 ? 0 : Top);
                if (this.IsFixed) {
                    if (this.ShowInterval)
                        clearTimeout(this.ShowInterval);
                    this.ShowInterval = setTimeout(this.ShowFunction, this.Timeout);
                }
            }
            if (this.IsEsc)
                fastCSharp.HtmlElement.$(document.body).AddEvent('keydown', this.KeyDownFunction);
        };
        FloatCenter.prototype.Hide = function () {
            fastCSharp.HtmlElement.$(document.body).DeleteEvent('keypress', this.KeyDownFunction);
            var Element = fastCSharp.HtmlElement.$Id(this.Id);
            if (fastCSharp.Pub.IsFixed) {
                if (this.AutoClass) {
                    Element.RemoveClass(this.AutoClass);
                    Element.Style('zIndex', -100);
                }
                else
                    Element.Display(0);
                fastCSharp.OverDiv.Default.Hide(this.Id);
            }
            else {
                if (this.ShowInterval)
                    clearTimeout(this.ShowInterval);
                this.ShowInterval = null;
                Element.Display(0);
                fastCSharp.OverDiv.Default.Hide();
            }
        };
        FloatCenter.prototype.KeyDown = function (Event) {
            if (Event.keyCode == 27) {
                var Element = Event.$Name('floatcenter');
                if (Element && Element.id == this.Id)
                    this.Hide();
            }
        };
        FloatCenter.GetFloatCenters = function () {
            if (!this.FloatCenters)
                fastCSharp.Pub.OnLoadedHash.Add(fastCSharp.Pub.ThisFunction(this, this.GetFloatCenters));
            this.FloatCenters = {};
            for (var Childs = fastCSharp.HtmlElement.$('@floatcenter').GetElements(), Index = Childs.length; --Index >= 0;) {
                var Element = Childs[Index], Parameter = fastCSharp.HtmlElement.$Attribute(Element, 'floatcenter');
                (Parameter = Parameter ? eval('(' + Parameter + ')') : {}).Id = Element.id;
                this.FloatCenters[Element.id] = new FloatCenter(Parameter);
            }
        };
        FloatCenter.DefaultParameter = { Id: null, IsFixed: false, IsEsc: true, Timeout: 200, SkinView: null, ZIndex: 0, AutoClass: null };
        return FloatCenter;
    }());
    fastCSharp.FloatCenter = FloatCenter;
    fastCSharp.Pub.OnLoad(FloatCenter.GetFloatCenters, FloatCenter, true);
})(fastCSharp || (fastCSharp = {}));
