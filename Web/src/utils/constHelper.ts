import type { App } from 'vue';
import { useUserInfo } from '/@/stores/userInfo';

export function setupConstFilter(app: App) {
	// 全局过滤器  在vue文件中调用  $filters.codeToName(code,type)
	app.config.globalProperties.$filters = {
		codeToName(code: any, type: any) {
			return codeToName(code, type);
		},
	};
}

// 常量值转换
export function codeToName(code: any, type: any) {
	const userStore = useUserInfo();
	try {
		const name = userStore.constList
			.filter((x: any) => x.code === type)
			.map((x: any) => x.data)
			.map((x: any) => x.filter((y: any) => y.code === code))
			.map((x: any) => x[0].name);
		return name[0];
	} catch (error) {
		return code;
	}
}

export function getConstType(type: any) {
	const userStore = useUserInfo();
	const constType = userStore.constList.filter((x: any) => x.code === type)[0].data;
	return constType;
}
