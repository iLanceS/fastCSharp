/// <reference path = "./hideButton.ts" />
/*include:include\hideButton*/
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
