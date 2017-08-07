/// <reference path = "../../ui/js/poll.ts" />
/// <reference path = "../../ui/js/htmlEditor.page.ts" />
/// <reference path = "./viewJs/api.ts" />
/*include:js\poll*/
'use strict';
interface IChatReturn extends fastCSharp.IPollReturn {
    Message: IChatReturnMessage;
}
interface IChatReturnMessage {
    UserVersion: number;
    Users: string[];
    Messages: any[];
}
interface ILoginReturn {
    version: number;
    __AJAXRETURN__: string[];
}
interface IUser {
    User: string;
    IsSelected: boolean;
}
class Chat {
    static Poll: fastCSharp.Poll;
    static User: string;
    static Load() {
        var PollParameter = new fastCSharp.PollParameter(), PollEvents = PollParameter as Object as fastCSharp.IPollEvents;
        PollParameter.VerifyPath = '';
        PollEvents.OnMessage = fastCSharp.Pub.ThisFunction(this, this.OnPoll);
        this.Poll = new fastCSharp.Poll(PollParameter);
        this.Login();
    }
    static Login() {
        this.User = fastCSharp.HtmlElement.$Id('User').Value0().Trim();
        if (this.User) {
            fastCSharp.Skin.BodyData('LoginUser').$Set(this.User);
            fastCSharp.Skin.BodyData('IsLogining').$Set(1);
            fastCSharpAPI.ajax.user.Login(this.User, 0, fastCSharp.Pub.ThisFunction(this, this.OnLogin));
        }
    }
    static OnLogin(Value: ILoginReturn) {
        fastCSharp.Skin.BodyData('IsLogining').$Set(0);
        if (Value.__AJAXRETURN__) {
            this.Poll.Query['UserVersion'] = Value.version;
            fastCSharp.Skin.BodyData('LoginUser').$Set(this.User);
            fastCSharp.Skin.BodyData('IsLogin').$Set(1);
            this.SetUsers(Value.__AJAXRETURN__);
            fastCSharp.Skin.Refresh();
            var HtmlEditor = fastCSharp.Declare.Getters['HtmlEditor']('MessageEditor', false) as fastCSharp.HtmlEditor;
            HtmlEditor.SetHtml('大家好，我是 ' + this.User);
            this.Send();
            this.Poll.Start(this.User);
        }
        else if (fastCSharp.HttpRequest.CheckError(Value))
        {
            alert('用户已注册 ' + this.User);
            fastCSharp.Skin.BodyData('LoginUser').$Set('');
        }
    }
    static SelectAllUser(IsSelect: boolean) {
        fastCSharp.Skin.BodyData('Users')
            .$For(function (Data) { Data.$('IsSelected').$Set(IsSelect); });
    }
    static ChangeSelectAllUser() {
        fastCSharp.Skin.BodyData('Users')
            .$For(function (Data) { Data.$('IsSelected').$Not(); });
    }
    static ChangeSelectUser(User: string) {
        fastCSharp.Skin.BodyData('Users')
            .$Find(function (Data: IUser) { return Data.User == User; })
            .For(function (Data) { Data.$('IsSelected').$Not(); });
    }
    static SetUsers(Users: string[]) {
        var SkinUsers = fastCSharp.Skin.BodyData('Users');
        var SelectedUsers = SkinUsers.$Data ? (SkinUsers.$Data as IUser[]).Find(function (User) { return User.IsSelected; }).ToHash(function (User) { return User.User }) : {};
        SkinUsers.$Set(Users.RemoveAt(Users.indexOf(this.User)).ToArray(function (User): IUser { return { User: User, IsSelected: !!SelectedUsers[User] } }));
    }
    static OnPoll(Value: IChatReturn) {
        if (this.User && Value.Message) {
            if (Value.Message.Users) {
                this.Poll.Query['UserVersion'] = Value.Message.UserVersion;
                this.SetUsers(Value.Message.Users);
            }
            fastCSharp.Skin.BodyData('Messages').$Pushs(Value.Message.Messages);
        }
    }
    static Logout() {
        fastCSharpAPI.ajax.user.Logout(this.User);
        this.Poll.Stop();
        this.User = null;
        var SkinData = fastCSharp.Skin.BodyData();
        SkinData.$('Users').$Set([]);
        SkinData.$('Messages').$Set([]);
        SkinData.$('LoginUser').$Set('');
        SkinData.$('IsLogin').$Set(0);
        SkinData.$('IsLogining').$Set(0);
    }
    static Send() {
        var SelectedUsers = fastCSharp.Skin.BodyData('Users').$Find(function (User: IUser) { return User.IsSelected; });
        var HtmlEditor = fastCSharp.Declare.Getters['HtmlEditor']('MessageEditor', false) as fastCSharp.HtmlEditor, Message = HtmlEditor.GetHtml();
        fastCSharpAPI.ajax.user.Send(this.User, Message, (SelectedUsers.length ? SelectedUsers.ToArray(function (Data) { return (Data.$Data as IUser).User; }) : null));
        fastCSharp.Skin.BodyData('Messages').$Push({ User: this.User, Time: new Date, Message: Message });
        HtmlEditor.SetHtml('');
    }
}
fastCSharp.Pub.OnModule(['htmlEditor'], fastCSharp.Pub.ThisFunction(Chat, Chat.Load), true);