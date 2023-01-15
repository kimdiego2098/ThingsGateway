<template>
	<el-form ref="ruleFormRef" :model="state.ruleForm" size="large" :rules="state.rules" class="login-content-form">
		<el-form-item class="login-animation1" prop="account">
			<el-input text placeholder="请输入账号" v-model="state.ruleForm.account" clearable autocomplete="off">
				<template #prefix>
					<el-icon>
						<ele-User />
					</el-icon>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation2" prop="password">
			<el-input :type="state.isShowPassword ? 'text' : 'password'" placeholder="请输入密码" v-model="state.ruleForm.password" autocomplete="off">
				<template #prefix>
					<el-icon>
						<ele-Unlock />
					</el-icon>
				</template>
				<template #suffix>
					<i class="iconfont el-input__icon login-content-password" :class="state.isShowPassword ? 'icon-yincangmima' : 'icon-xianshimima'" @click="state.isShowPassword = !state.isShowPassword"> </i>
				</template>
			</el-input>
		</el-form-item>
		<el-form-item class="login-animation3" prop="captcha" v-show="state.captchaEnabled">
			<el-col :span="15">
				<el-input text maxlength="4" :placeholder="$t('message.account.accountPlaceholder3')" v-model="state.ruleForm.code" clearable autocomplete="off">
					<template #prefix>
						<el-icon>
							<ele-Position />
						</el-icon>
					</template>
				</el-input>
			</el-col>
			<el-col :span="1"></el-col>
			<el-col :span="8">
				<div class="login-content-code">
					<img class="login-content-code-img" @click="getCaptcha" width="130px" height="38px" :src="state.captchaImage" style="cursor: pointer" />
				</div>
			</el-col>
		</el-form-item>
		<el-form-item class="login-animation4">
			<el-button type="primary" class="login-content-submit" round v-waves @click="state.secondVerEnabled ? openRotateVerify() : onSignIn()" :loading="state.loading.signIn">
				<span>{{ $t('message.account.accountBtnText') }}</span>
			</el-button>
		</el-form-item>
		<div class="font12 mt30 login-animation4 login-msg">{{ $t('message.mobile.msgText') }}</div>
	</el-form>

	<div class="dialog-header">
		<el-dialog v-model="state.rotateVerifyVisible" :show-close="false">
			<DragVerifyImgRotate
				ref="dragRef"
				:imgsrc="state.rotateVerifyImg"
				v-model:isPassing="state.isPassRotate"
				text="请按住滑块拖动"
				successText="验证通过"
				handlerIcon="fa fa-angle-double-right"
				successIcon="fa fa-hand-peace-o"
				@passcallback="passRotateVerify"
			/>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="loginAccount">
import { reactive, computed, ref, onMounted, defineAsyncComponent } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { useI18n } from 'vue-i18n';
import { initBackEndControlRoutes } from '/@/router/backEnd';
import { Local, Session } from '/@/utils/storage';
import { formatAxis } from '/@/utils/formatTime';
import { NextLoading } from '/@/utils/loading';

import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';
import Watermark from '/@/utils/wartermark';

import { clearTokens, feature, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi } from '/@/api-services/api';

// 旋转图片滑块组件
import verifyImg from '/@/assets/logo-mini.svg';
const DragVerifyImgRotate = defineAsyncComponent(() => import('/@/components/dragVerify/dragVerifyImgRotate.vue'));

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const ruleFormRef = ref();
const dragRef: any = ref(null);
const state = reactive({
	isShowPassword: false,
	ruleForm: {
        account: 'admin',
		password: '123456',
		code: '',
		codeId: 0,
	},
	rules: {
		account: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
		password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
		code: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
	},
	loading: {
		signIn: false,
	},
	captchaImage: '',
	rotateVerifyVisible: false,
	rotateVerifyImg: verifyImg,
	secondVerEnabled: true,
	captchaEnabled: true,
	wartermarkEnabled: true,
	isPassRotate: false,
});
onMounted(async () => {
	// 获取登录配置
	var res1 = await getAPI(SysAuthApi).apiSysAuthGetLoginConfigGet();
	state.secondVerEnabled = res1.data.result.secondVerEnabled ?? true;
	state.captchaEnabled = res1.data.result.captchaEnabled ?? true;
	state.wartermarkEnabled = res1.data.result.wartermarkEnabled ?? true;

	getCaptcha();
});
// 获取布局配置信息
const getThemeConfig = computed(() => {
	return themeConfig.value;
});
// 存储布局配置
const setLocalThemeConfig = () => {
	// 是否显示水印
	if (!state.wartermarkEnabled) return;

	// getThemeConfig.value.wartermarkText = state.ruleForm.account;
	getThemeConfig.value.isWartermark = true;
	Watermark.set(getThemeConfig.value.wartermarkText);

	Local.remove('themeConfig');
	Local.set('themeConfig', getThemeConfig.value);
};
// 获取验证码
const getCaptcha = async () => {
	state.ruleForm.code = '';
	var res = await getAPI(SysAuthApi).apiSysAuthGetCaptchaGet();
	state.captchaImage = 'data:text/html;base64,' + res.data.result?.img;
	state.ruleForm.codeId = res.data.result?.id;
};
// 获取时间
const currentTime = computed(() => {
	return formatAxis(new Date());
});
// 登录
const onSignIn = async () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return false;

		const [err, res] = await feature(getAPI(SysAuthApi).apiSysAuthLoginPost(state.ruleForm));
		if (err) {
			getCaptcha(); // 重新获取验证码
			return;
		}
		if (res.data.result?.accessToken == undefined) {
			getCaptcha(); // 重新获取验证码
			ElMessage.error('登录失败，请检查账号！');
			return;
		}

		state.loading.signIn = true;
		Session.set('token', res.data.result?.accessToken); // 缓存token
		// 添加完动态路由再进行router跳转，否则可能报错 No match found for location with path "/"
		const isNoPower = await initBackEndControlRoutes();
		signInSuccess(isNoPower); // 再执行 signInSuccess
	});
};
// 登录成功后的跳转
const signInSuccess = (isNoPower: boolean | undefined) => {
	if (isNoPower) {
		ElMessage.warning('抱歉，您没有登录权限');
		clearTokens(); // 清空Token缓存
	} else {
		// 初始化登录成功时间问候语
		let currentTimeInfo = currentTime.value;
		// 登录成功，跳到转首页 如果是复制粘贴的路径，非首页/登录页，那么登录成功后重定向到对应的路径中
		if (route.query?.redirect) {
			router.push({
				path: <string>route.query?.redirect,
				query: Object.keys(<string>route.query?.params).length > 0 ? JSON.parse(<string>route.query?.params) : '',
			});
		} else {
			router.push('/');
		}
		// // 设置水印
		// setLocalThemeConfig();

		// 登录成功提示
		const signInText = t('message.signInText');
		ElMessage.success(`${currentTimeInfo}，${signInText}`);
		// 添加 loading，防止第一次进入界面时出现短暂空白
		NextLoading.start();
	}
	state.loading.signIn = false;
};
// 打开旋转验证
const openRotateVerify = () => {
	state.rotateVerifyVisible = true;
	state.isPassRotate = false;
	dragRef.value?.reset();
};
// 通过旋转验证
const passRotateVerify = () => {
	state.rotateVerifyVisible = false;
	state.isPassRotate = true;
	onSignIn();
};
</script>

<style scoped lang="scss">
.dialog-header {
	:deep(.el-dialog) {
		.el-dialog__header {
			display: none;
		}

		.el-dialog__wrapper {
			position: absolute !important;
		}

		.v-modal {
			position: absolute !important;
		}

		width: unset !important;
	}
}

.login-content-form {
	margin-top: 20px;

	@for $i from 0 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}

	.login-content-password {
		display: inline-block;
		width: 20px;
		cursor: pointer;

		&:hover {
			color: #909399;
		}
	}

	.login-content-code {
		display: flex;
		align-items: center;
		justify-content: space-around;

		.login-content-code-img {
			width: 100%;
			height: 40px;
			line-height: 40px;
			background-color: #ffffff;
			border: 1px solid rgb(220, 223, 230);
			cursor: pointer;
			transition: all ease 0.2s;
			border-radius: 4px;
			user-select: none;

			&:hover {
				border-color: #c0c4cc;
				transition: all ease 0.2s;
			}
		}
	}

	.login-content-submit {
		width: 100%;
		letter-spacing: 2px;
		font-weight: 300;
		margin-top: 15px;
	}

	.login-msg {
		color: var(--el-text-color-placeholder);
	}
}
</style>
