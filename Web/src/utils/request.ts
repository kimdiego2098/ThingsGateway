import axios, { AxiosInstance } from 'axios';
import { ElMessage } from 'element-plus';
import { Local, Session } from '/@/utils/storage';

// 配置新建一个 axios 实例
export const service = axios.create({
	baseURL: (window as any).baseUrl.baseUrl as any,
	timeout: 50000,
	headers: { 'Content-Type': 'application/json' },
});

// token 键定义
export const accessTokenKey = 'access-token';
export const refreshAccessTokenKey = `x-${accessTokenKey}`;

// 获取 token
export const getToken = () => {
	return Local.get(accessTokenKey);
};

// 清除 token
export const clearAccessTokens = () => {
	Local.remove(accessTokenKey);
	Local.remove(refreshAccessTokenKey);

	// 清除其他
	Session.clear();

	// 刷新浏览器
	window.location.reload();
};

// axios 默认实例
export const axiosInstance: AxiosInstance = axios;

// 添加请求拦截器
service.interceptors.request.use(
	(config) => {
		// // 在发送请求之前做些什么 token
		// if (Session.get('token')) {
		// 	(<any>config.headers).common['Authorization'] = `${Session.get('token')}`;
		// }

		// 获取本地的 token
		const accessToken = Local.get(accessTokenKey);
		if (accessToken) {
			// 将 token 添加到请求报文头中
			config.headers!['Authorization'] = `Bearer ${accessToken}`;

			// 判断 accessToken 是否过期
			const jwt: any = decryptJWT(accessToken);
			const exp = getJWTDate(jwt.exp as number);

			// token 已经过期
			if (new Date() >= exp) {
				// 获取刷新 token
				const refreshAccessToken = Local.get(refreshAccessTokenKey);

				// 携带刷新 token
				if (refreshAccessToken) {
					config.headers!['X-Authorization'] = `Bearer ${refreshAccessToken}`;
				}
			}
			// debugger
			// get请求映射params参数
			if (config.method?.toLowerCase() === 'get' && config.data) {
				let url = config.url + '?' + tansParams(config.data);
				url = url.slice(0, -1);
				config.data = {};
				config.url = url;
			}
		}
		return config;
	},
	(error) => {
		// 对请求错误做些什么
		return Promise.reject(error);
	}
);

// 添加响应拦截器
service.interceptors.response.use(
	(res) => {
		// 获取状态码和返回数据
		var status = res.status;
		var serve = res.data;

		// 处理 401
		if (status === 401) {
			clearAccessTokens();
		}

		// 处理未进行规范化处理的
		if (status >= 400) {
			throw new Error(res.statusText || 'Request Error.');
		}

		// 处理规范化结果错误
		if (serve && serve.hasOwnProperty('errors') && serve.errors) {
			throw new Error(JSON.stringify(serve.errors || 'Request Error.'));
		}

		// 读取响应报文头 token 信息
		var accessToken = res.headers[accessTokenKey];
		var refreshAccessToken = res.headers[refreshAccessTokenKey];

		// 判断是否是无效 token
		if (accessToken === 'invalid_token') {
			clearAccessTokens();
		}
		// 判断是否存在刷新 token，如果存在则存储在本地
		else if (refreshAccessToken && accessToken && accessToken !== 'invalid_token') {
			Local.set(accessTokenKey, accessToken);
			Local.set(refreshAccessTokenKey, refreshAccessToken);
		}

		// 响应拦截及自定义处理
		if (serve.code === 401) {
			clearAccessTokens();
		} else if (serve.code === undefined) {
			return Promise.resolve(res);
		} else if (serve.code !== 200) {
			var message = JSON.stringify(serve.message);
			ElMessage.error(message);
			throw new Error(message);
		}

		return res;
	},
	(error) => {
		// 处理响应错误
		if (error.response) {
			if (error.response.status === 401) {
				clearAccessTokens();
			}
		}

		// 对响应错误做点什么
		if (error.message.indexOf('timeout') != -1) {
			ElMessage.error('网络超时');
		} else if (error.message == 'Network Error') {
			ElMessage.error('网络连接错误');
		} else {
			if (error.response.data) ElMessage.error(error.response.statusText);
			else ElMessage.error('接口路径找不到');
		}

		return Promise.reject(error);
	}
);

/**
 *  参数处理
 * @param {*} params  参数
 */
export function tansParams(params: any) {
	let result = '';
	for (const propName of Object.keys(params)) {
		const value = params[propName];
		var part = encodeURIComponent(propName) + '=';
		if (value !== null && value !== '' && typeof value !== 'undefined') {
			if (typeof value === 'object') {
				for (const key of Object.keys(value)) {
					if (value[key] !== null && value[key] !== '' && typeof value[key] !== 'undefined') {
						let params = propName + '[' + key + ']';
						var subPart = encodeURIComponent(params) + '=';
						result += subPart + encodeURIComponent(value[key]) + '&';
					}
				}
			} else {
				result += part + encodeURIComponent(value) + '&';
			}
		}
	}
	return result;
}

/**
 * 解密 JWT token 的信息
 * @param token jwt token 字符串
 * @returns <any>object
 */
export function decryptJWT(token: string): any {
	token = token.replace(/_/g, '/').replace(/-/g, '+');
	var json = decodeURIComponent(escape(window.atob(token.split('.')[1])));
	return JSON.parse(json);
}

/**
 * 将 JWT 时间戳转换成 Date
 * @description 主要针对 `exp`，`iat`，`nbf`
 * @param timestamp 时间戳
 * @returns Date 对象
 */
export function getJWTDate(timestamp: number): Date {
	return new Date(timestamp * 1000);
}

// 导出 axios 实例
export default service;