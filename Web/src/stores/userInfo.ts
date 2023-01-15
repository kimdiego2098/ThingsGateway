import { defineStore } from 'pinia';
import { Session } from '/@/utils/storage';

import { getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysConstApi } from '/@/api-services/api';

/**
 * 用户信息
 * @methods setUserInfos 设置用户信息
 */
export const useUserInfo = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {} as any,
		constList: [] as any,
	}),
	getters: {
		// 获取系统常量列表
		async getSysConstList(): Promise<any[]> {
			var res = await getAPI(SysConstApi).apiSysConstGetConstListGet();
			this.constList = res.data.result ?? [];
			return this.constList;
		},
	},
	actions: {
		async setUserInfos() {
			// 缓存用户信息
			if (Session.get('userInfo')) {
				this.userInfos = Session.get('userInfo');
			} else {
				const userInfos: any = await this.getApiUserInfo();
				this.userInfos = userInfos;
			}
		},
		// 获取当前用户信息
		async getApiUserInfo() {
			return new Promise((resolve) => {
				getAPI(SysAuthApi)
					.apiSysAuthGetUserInfoGet()
					.then((res: any) => {
						if (res.data.result == null) return;
						var d = res.data.result;
						const userInfos = {
							account: d.account,
							realName: d.realName,
							avatar: d.avatar ? d.avatar : '/favicon.ico',
							address: d.address,
							signature: d.signature,
							orgId: d.orgId,
							orgName: d.orgName,
							posName: d.posName,
							roles: [],
							authBtnList: d.buttons,
							time: new Date().getTime(),
						};
						resolve(userInfos);
					});
			});
		},
	},
});
