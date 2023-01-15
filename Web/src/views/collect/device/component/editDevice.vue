<template>
	<div class="sys-device-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle">
						<ele-Edit />
					</el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-tabs v-loading="loading">

				<el-tab-pane label="基础信息">
					<el-form :model="deviceForm" ref="deviceFormRef" size="default" label-width="100px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="设备名称" prop="name"
									:rules="[{ required: true, message: '设备名称不能为空', trigger: 'blur' }]">
									<el-input v-model="deviceForm.name" placeholder="设备名称" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="描述" prop="description">
									<el-input v-model="deviceForm.description" placeholder="描述" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="驱动" prop="driverAssembleName"
									:rules="[{ required: true, message: '驱动不能为空', trigger: 'blur' }]">

									<el-cascader :options="driverpluginList" :show-all-levels="false"
										:props="{ emitPath: false, value: 'name', label: 'name' }" placeholder="驱动"
										clearable class="w100" v-model="deviceForm.driverAssembleName">

										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>

									</el-cascader>
									<span style="font-size: 12px; color: gray; padding-left: 5px"> 注意：更换驱动后需手动刷新设备属性并赋值
									</span>

								</el-form-item>
							</el-col>



							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="执行间隔" prop="invokeInterval"
									:rules="[{ required: true, message: '间隔不能为空', trigger: 'blur' }]">
									<el-input v-model="deviceForm.invokeInterval" placeholder="默认执行间隔" clearable />
								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="变化上传" prop="changeUpload">
									<el-radio-group v-model="deviceForm.changeUpload">
										<el-radio :label="true">启用</el-radio>
										<el-radio :label="false">停用</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>


						</el-row>
					</el-form>
				</el-tab-pane>

				<!-- <template > -->

					<el-tab-pane label="设备属性">

						<el-form :model="deviceForm" ref="deviceFormRef" size="default" label-width="150px">
							<el-row :gutter="35">
								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
										<el-button icon="ele-Refresh" type="primary" plain
											@click="reProperty(deviceForm)"> 刷新驱动属性 </el-button>
										<span style="font-size: 12px; color: gray; padding-left: 5px"> 刷新后属性值将被清空
										</span>
									</el-col>
									<template
										v-if="deviceForm.deviceProperties != undefined && deviceForm.deviceProperties.length > 0">

										<el-row :gutter="35" v-for="(v, k) in deviceForm.deviceProperties" :key="k">
											<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">

												<el-form-item :label="deviceForm.deviceProperties[k].devicePropertyName"
													style="width: 100%" :prop="`deviceProperties[${k}].value`"
													:rules="[{ required: false, message: `不能为空`, trigger: 'blur' }]">
													<el-input v-model="deviceForm.deviceProperties[k].value"
														:placeholder="deviceForm.deviceProperties[k].description"
														style="width: 100%" clearable />
												</el-form-item>

											</el-col>


										</el-row>
									</template>
									<el-empty description="空" v-else></el-empty>
								</el-col>
							</el-row>
						</el-form>
					</el-tab-pane>

				<!-- </template> -->
			</el-tabs>

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
import { reactive, toRefs, defineComponent, ref, onMounted } from 'vue';
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { DeviceApi, DriverPluginApi } from '/@/api-services/api';
import { Device } from '/@/api-services/models';


export default defineComponent({
	name: 'driverPluginEdit',
	components: {},
	props: {
		title: {
			type: String,
			default: '',
		},
		isAdd: {
			type: Boolean,
			default: true,
		}
	},
	setup() {
		const deviceFormRef = ref();
		const state = reactive({
			loading: false,
			isShowDialog: false,
			deviceForm: {} as Device,

			driverpluginList: [{
				type: Array<any>,
				default: () => [],
			},]

		});
		onMounted(async () => {
			state.loading = true;

			let resDicData = await getAPI(DriverPluginApi).apiDriverPluginGetDriverPluginInfosGet();
			state.driverpluginList = resDicData.data.result as Array<any>;

			state.loading = false;

		});


		// 打开弹窗
		const openDialog = async (row: any) => {
			state.deviceForm = JSON.parse(JSON.stringify(row));
			let resDicData = await getAPI(DriverPluginApi).apiDriverPluginGetDriverPluginInfosGet();
			state.driverpluginList = resDicData.data.result as Array<any>;
			state.isShowDialog = true;

			if (state.deviceForm.name == undefined)
				state.deviceForm.name = "dev";
			if (state.deviceForm.invokeInterval == undefined)
				state.deviceForm.invokeInterval = 1000;
			if (state.deviceForm.changeUpload == undefined)
				state.deviceForm.changeUpload = true;
		};
		// 关闭弹窗
		const closeDialog = () => {
			mittBus.emit('deviceRefresh');
			state.isShowDialog = false;
		};
		// 取消
		const cancel = () => {
			state.isShowDialog = false;
		};
		// 提交
		const submit = () => {
			deviceFormRef.value.validate(async (valid: boolean) => {
				if (!valid) return;
				if (state.deviceForm.id != undefined && state.deviceForm.id > 0) {
					await getAPI(DeviceApi).apiDeviceUpdateConfigPost(state.deviceForm);
				} else {
					await getAPI(DeviceApi).apiDeviceAddDevicePost(state.deviceForm);
				}
				closeDialog();
			});
		};
		// 刷新设备属性
		const reProperty = async (row: any) => {
			if (JSON.stringify(row) !== '{}') {
				var resRole = await getAPI(DeviceApi).apiDeviceRefreshDevicePropertyPost(row);
				state.deviceForm.deviceProperties = resRole.data.result;
			}
		};


		return {
			deviceFormRef,
			openDialog,
			closeDialog,
			cancel,
			reProperty,
			submit,
			...toRefs(state),
		};
	},
});
</script>
