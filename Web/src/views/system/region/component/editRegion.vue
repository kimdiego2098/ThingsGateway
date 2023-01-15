<template>
	<div class="sys-region-container">
		<el-dialog v-model="isShowDialog" draggable width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="80px">
				<el-row :gutter="35">
					<!-- <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="上级名称">
							<el-cascader
								:options="regionData"
								:props="{ checkStrictly: true, emitPath: false, value: 'id', label: 'name' }"
								placeholder="请选择上级名称"
								clearable
								class="w100"
								v-model="ruleForm.pid"
							>
								<template #default="{ node, data }">
									<span>{{ data.name }}</span>
									<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
								</template>
							</el-cascader>
						</el-form-item>
					</el-col> -->
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="行政名称" prop="name" :rules="[{ required: true, message: '行政名称不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.name" placeholder="行政名称" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="行政代码" prop="code" :rules="[{ required: true, message: '行政代码不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.code" placeholder="行政代码" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="区号" prop="cityCode">
							<el-input v-model="ruleForm.cityCode" placeholder="区号" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="排序">
							<el-input-number v-model="ruleForm.orderNo" placeholder="排序" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="备注">
							<el-input v-model="ruleForm.remark" placeholder="请输入备注内容" clearable type="textarea" />
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
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { SysRegionApi } from '/@/api-services/api';
import { UpdateRegionInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditRegion',
	components: {},
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
			ruleForm: {} as UpdateRegionInput,
		});
		// 打开弹窗
		const openDialog = (row: any) => {
			state.ruleForm = JSON.parse(JSON.stringify(row));
			state.isShowDialog = true;
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
					await getAPI(SysRegionApi).apiSysRegionUpdateRegionPost(state.ruleForm);
				} else {
					await getAPI(SysRegionApi).apiSysRegionAddRegionPost(state.ruleForm);
				}
				mittBus.emit('submitRefresh');
				state.isShowDialog = false;
			});
		};
		return {
			ruleFormRef,
			openDialog,
			cancel,
			submit,
			...toRefs(state),
		};
	},
});
</script>
