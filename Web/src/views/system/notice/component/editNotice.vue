<template>
	<div class="sys-notice-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="800px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-form :model="ruleForm" ref="ruleFormRef" size="default">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="标题" prop="title" :rules="[{ required: true, message: '标题不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.title" placeholder="标题" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="类型" prop="type" :rules="[{ required: true, message: '类型不能为空', trigger: 'blur' }]">
							<el-select v-model="ruleForm.type" placeholder="类型" filterable allow-create default-first-option style="width: 100%">
								<el-option label="通知" :value="1" />
								<el-option label="公告" :value="2" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="内容" prop="content" :rules="[{ required: true, message: '内容不能为空', trigger: 'blur' }]">
							<Editor v-model:get-html="ruleForm.content" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel" size="default">取 消</el-button>
					<el-button type="primary" @click="submit" size="default">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts">
import { reactive, toRefs, defineComponent, ref } from 'vue';
import Editor from '/@/components/editor/index.vue';
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { UpdateNoticeInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysNoticeEdit',
	components: { Editor },
	props: {
		title: {
			type: String,
			default: '',
		},
	},
	setup() {
		const ruleFormRef = ref();
		const state = reactive({
			isShowDialog: false,
			ruleForm: {} as UpdateNoticeInput,
		});
		// 打开弹窗
		const openDialog = (row: any) => {
			state.ruleForm = JSON.parse(JSON.stringify(row));
			state.isShowDialog = true;
		};
		// 关闭弹窗
		const closeDialog = () => {
			mittBus.emit('submitRefresh');
			state.isShowDialog = false;
		};
		// 取消
		const cancel = () => {
			state.isShowDialog = false;
		};
		// 提交
		const submit = () => {
			ruleFormRef.value.validate(async (valid: boolean) => {
				if (!valid) return;
				if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
					await getAPI(SysNoticeApi).apiSysNoticeUpdateNoticePost(state.ruleForm);
				} else {
					await getAPI(SysNoticeApi).apiSysNoticeAddNoticePost(state.ruleForm);
				}
				closeDialog();
			});
		};
		return {
			ruleFormRef,
			openDialog,
			closeDialog,
			cancel,
			submit,
			...toRefs(state),
		};
	},
});
</script>
