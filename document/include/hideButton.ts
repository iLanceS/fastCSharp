/// <reference path = "./mark.ts" />
class HideButton {
    private static ChangeNoCookie(Name: string) {
        fastCSharp.Skin.BodyData('IsHide' + Name).$Not();
    }
    private static SetCookie(Name: string, IsHide: boolean) {
        var Value = fastCSharp.Cookie.Default.Read('HideButton');
        if (IsHide) Value = Value ? Value.split(',').RemoveValue(Name).Push(Name).join(',') : Name;
        else Value = Value ? Value.split(',').RemoveValue(Name).join(',') : '';
        fastCSharp.Cookie.Default.Write({ Name: 'HideButton', Value: Value } as fastCSharp.ICookieValue);
    }
    private static Change(Name: string) {
        var IsHide = fastCSharp.Skin.BodyData('IsHide' + Name).$Not();
        if (IsHide) this.SetCookie(Name, IsHide);
        else Mark.To(Name, true, false);
    }
    static TryShow(Name: string, IsShow = true, IsCookie = true) {
        if (fastCSharp.Skin.Body.Data['IsHide' + Name] ? IsShow : !IsShow) {
            fastCSharp.Skin.BodyData('IsHide' + Name).$Set(!IsShow);
            if (IsCookie) this.SetCookie(Name, !IsShow);
        }
    }
    private static LoadEvents: fastCSharp.Events = new fastCSharp.Events();
    static OnLoad(Function: Function) {
        if (this.LoadEvents) this.LoadEvents.Add(Function);
        else Function();
    }
    static Load() {
        var Name = fastCSharp.Cookie.Default.Read('HideButton');
        if (Name) {
            for (var Names = Name.split(','), Index = Names.length; Index; this.TryShow(Names[--Index], false, false));
        }
        this.LoadEvents.Function();
        this.LoadEvents = null;
    }
}
fastCSharp.Pub.OnLoad(HideButton.Load, HideButton, true);