/// <reference path = "./mark.ts" />
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
