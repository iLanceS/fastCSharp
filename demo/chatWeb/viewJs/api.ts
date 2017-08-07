//本文件由程序自动生成,请不要自行修改
module fastCSharpAPI.ajax {
	export class user {
		
		static Login(user,version,Callback = null) {
			fastCSharp.Pub.GetAjaxPost()('user.Login',{user: user, version: version }, Callback);	
		}
		
		static Logout(user,Callback = null) {
			fastCSharp.Pub.GetAjaxPost()('user.Logout',{user: user }, Callback);	
		}
		
		static Send(user,message,users,Callback = null) {
			fastCSharp.Pub.GetAjaxPost()('user.Send',{user: user, message: message, users: users }, Callback);	
		}
		
	}
}