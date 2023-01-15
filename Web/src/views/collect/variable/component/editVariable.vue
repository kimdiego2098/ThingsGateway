<template>
	<div class="sys-variable-container">
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
					<el-form :model="deviceVariableForm" ref="deviceVariableFormRef" size="default" label-width="100px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="变量名称" prop="name"
									:rules="[{ required: true, message: '变量名称不能为空', trigger: 'blur' }]">
									<el-input v-model="deviceVariableForm.name" placeholder="变量名称" clearable />
								</el-form-item>
							</el-col>


							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="设备" prop="deviceId"
									:rules="[{ required: true, message: '设备不能为空', trigger: 'blur' }]">
									<el-cascader :options="deviceNameList"
										@change="deviceChange(deviceVariableForm.deviceId)" :show-all-levels="false"
										:props="{ emitPath: false, value: 'id', label: 'name' }" placeholder="设备"
										clearable class="w100" v-model="deviceVariableForm.deviceId">
										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>

								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="描述" prop="description">
									<el-input v-model="deviceVariableForm.description" placeholder="描述" clearable />
								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="数据类型" prop="coreDataType"
									:rules="[{ required: true, message: '数据类型不能为空', trigger: 'blur' }]">

									<el-cascader :options="dataTypeList" :show-all-levels="false"
										:props="{ emitPath: false, value: 'value', label: 'label' }" placeholder="数据类型"
										clearable class="w100" v-model="deviceVariableForm.coreDataType">
										<template #default="{ node, data }">
											<span>{{ data.label }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>

								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="读写权限" prop="coreDataType"
									:rules="[{ required: true, message: '读写权限不能为空', trigger: 'blur' }]">

									<el-cascader :options="protectTypeList" :show-all-levels="false"
										:props="{ emitPath: false, value: 'value', label: 'label' }" placeholder="读写权限"
										clearable class="w100" v-model="deviceVariableForm.protectType">
										<template #default="{ node, data }">
											<span>{{ data.label }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>

								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="初始值" prop="initialValue">
									<el-input v-model="deviceVariableForm.initialValue" placeholder="初始值" clearable />
								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="执行间隔" prop="invokeInterval"
									:rules="[{ required: true, message: '间隔不能为空', trigger: 'blur' }]">
									<el-input v-model="deviceVariableForm.invokeInterval" placeholder="执行间隔"
										clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="执行参数" prop="variableAddress">
									<el-input v-model="deviceVariableForm.variableAddress" placeholder="执行参数"
										clearable />
								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="其他方法" prop="otherMethod">

									<el-cascader :options="deviceMedsList" :show-all-levels="false"
										:props="{ emitPath: false, value: 'name', label: 'name' }" placeholder="其他方法"
										clearable class="w100" v-model="deviceVariableForm.otherMethod">
										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>

								</el-form-item>
							</el-col>

							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="表达式" prop="expressions">
									<el-input v-model="deviceVariableForm.expressions" placeholder="表达式" clearable />
								</el-form-item>
							</el-col>




						</el-row>
					</el-form>
				</el-tab-pane>


				<el-tab-pane label="报警属性">
					<el-form :model="deviceVariableForm" ref="deviceVariableFormRef" size="default" label-width="150px">
						<el-row :gutter="35">
							<el-col v-if="(deviceVariableForm.variableAlarms == undefined)" :xs="24" :sm="24" :md="24"
								:lg="24" :xl="24" class="mb20">
								<el-button icon="ele-Refresh" type="primary" plain @click="reAlarm(deviceVariableForm)">
									添加报警属性 </el-button>
							</el-col>
							<template v-if="deviceVariableForm.variableAlarms != undefined">

								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="报警组" style="width: 100%" :prop="`variableAlarms.alarmGroup`">
										<el-input v-model="deviceVariableForm.variableAlarms.alarmGroup"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="报警死区" style="width: 100%"
										:prop="`variableAlarms.alarmDeadZone`">
										<el-input v-model="deviceVariableForm.variableAlarms.alarmDeadZone"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="报警延时" style="width: 100%"
										:prop="`variableAlarms.alarmDelayTime`">
										<el-input v-model="deviceVariableForm.variableAlarms.alarmDelayTime"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-divider />

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="开报警使能" style="width: 120%"
										:prop="`variableAlarms.boolOpenAlarmEnable`">
										<el-radio-group v-model="deviceVariableForm.variableAlarms.boolOpenAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="开报警文本" style="width: 100%"
										:prop="`variableAlarms.boolOpenAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.boolOpenAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="开报警约束" style="width: 100%"
										:prop="`variableAlarms.boolOpenRestrainExpressions`">
										<el-input
											v-model="deviceVariableForm.variableAlarms.boolOpenRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-divider />

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="关报警使能" style="width: 120%"
										:prop="`variableAlarms.boolCloseAlarmEnable`">
										<el-radio-group
											v-model="deviceVariableForm.variableAlarms.boolCloseAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="关报警文本" style="width: 100%"
										:prop="`variableAlarms.boolCloseAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.boolCloseAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="关报警约束" style="width: 100%"
										:prop="`variableAlarms.boolCloseRestrainExpressions`">
										<el-input
											v-model="deviceVariableForm.variableAlarms.boolCloseRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高高报警使能" style="width: 120%"
										:prop="`variableAlarms.hhAlarmEnable`">
										<el-radio-group v-model="deviceVariableForm.variableAlarms.hhAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高高报警文本" style="width: 100%"
										:prop="`variableAlarms.hhAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.hhAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高高报警值" style="width: 100%"
										:prop="`variableAlarms.hhAlarmCode`">
										<el-input v-model="deviceVariableForm.variableAlarms.hhAlarmCode"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高高报警约束" style="width: 100%"
										:prop="`variableAlarms.hhRestrainExpressions`">
										<el-input v-model="deviceVariableForm.variableAlarms.hhRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高报警使能" style="width: 120%"
										:prop="`variableAlarms.hAlarmEnable`">
										<el-radio-group v-model="deviceVariableForm.variableAlarms.hAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高报警文本" style="width: 100%" :prop="`variableAlarms.hAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.hAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高报警值" style="width: 100%" :prop="`variableAlarms.hAlarmCode`">
										<el-input v-model="deviceVariableForm.variableAlarms.hAlarmCode"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="高报警约束" style="width: 100%"
										:prop="`variableAlarms.hRestrainExpressions`">
										<el-input v-model="deviceVariableForm.variableAlarms.hRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />


								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低报警使能" style="width: 120%"
										:prop="`variableAlarms.lAlarmEnable`">
										<el-radio-group v-model="deviceVariableForm.variableAlarms.lAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低报警文本" style="width: 100%" :prop="`variableAlarms.lAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.lAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低报警值" style="width: 100%" :prop="`variableAlarms.lAlarmCode`">
										<el-input v-model="deviceVariableForm.variableAlarms.lAlarmCode"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低报警约束" style="width: 100%"
										:prop="`variableAlarms.lRestrainExpressions`">
										<el-input v-model="deviceVariableForm.variableAlarms.lRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低低报警使能" style="width: 120%"
										:prop="`variableAlarms.llAlarmEnable`">
										<el-radio-group v-model="deviceVariableForm.variableAlarms.llAlarmEnable">
											<el-radio :label="true">启用</el-radio>
											<el-radio :label="false">停用</el-radio>
										</el-radio-group>
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低低报警文本" style="width: 100%"
										:prop="`variableAlarms.llAlarmText`">
										<el-input v-model="deviceVariableForm.variableAlarms.llAlarmText"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低低报警值" style="width: 100%"
										:prop="`variableAlarms.llAlarmCode`">
										<el-input v-model="deviceVariableForm.variableAlarms.llAlarmCode"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="低低报警约束" style="width: 100%"
										:prop="`variableAlarms.llRestrainExpressions`">
										<el-input v-model="deviceVariableForm.variableAlarms.llRestrainExpressions"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />

							</template>
							<el-empty description="空" v-else></el-empty>
						</el-row>
					</el-form>
				</el-tab-pane>


				<el-tab-pane label="历史属性">
					<el-form :model="deviceVariableForm" ref="deviceVariableFormRef" size="default" label-width="150px">
						<el-row :gutter="35">
							<el-col v-if="(deviceVariableForm.variableHiss == undefined)" :xs="24" :sm="24" :md="24"
								:lg="24" :xl="24" class="mb20">
								<el-button icon="ele-Refresh" type="primary" plain @click="reHis(deviceVariableForm)">
									添加历史属性 </el-button>
							</el-col>
							<template v-if="deviceVariableForm.variableHiss != undefined">


								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="启用" prop="enable">
										<el-select v-model="deviceVariableForm.variableHiss.enable" style="width: 100%">
											<el-option :label="true" :value="true" />
											<el-option :label="false" :value="false" />
										</el-select>
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item prop="hisType" label="存储类型">
										<el-select v-model="deviceVariableForm.variableHiss.hisType" placeholder="数据库类型"
											style="width: 100%">
											<el-option v-for="d in hisTypes" :key="d.value" :label="d.label"
												:value="d.value" />
										</el-select>
									</el-form-item>
								</el-col>



							</template>
							<el-empty description="空" v-else></el-empty>
						</el-row>
					</el-form>
				</el-tab-pane>


				<el-tab-pane label="扩展属性">
					<el-form :model="deviceVariableForm" ref="deviceVariableFormRef" size="default" label-width="150px">
						<el-row :gutter="35">
							<el-col v-if="(deviceVariableForm.variablePropertys == undefined)" :xs="24" :sm="24"
								:md="24" :lg="24" :xl="24" class="mb20">
								<el-button icon="ele-Refresh" type="primary" plain @click="reEx(deviceVariableForm)">
									添加扩展属性 </el-button>
							</el-col>
							<template v-if="deviceVariableForm.variablePropertys != undefined">
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域1" style="width: 100%"
										:prop="`variablePropertys.extendFieldString1`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString1"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域2" style="width: 100%"
										:prop="`variablePropertys.extendFieldString2`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString2"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域3" style="width: 100%"
										:prop="`variablePropertys.extendFieldString3`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString3"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域4" style="width: 100%"
										:prop="`variablePropertys.extendFieldString4`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString4"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域5" style="width: 100%"
										:prop="`variablePropertys.extendFieldString5`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString5"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域6" style="width: 100%"
										:prop="`variablePropertys.extendFieldString6`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString6"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域7" style="width: 100%"
										:prop="`variablePropertys.extendFieldString7`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString7"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域8" style="width: 100%"
										:prop="`variablePropertys.extendFieldString8`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString8"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域9" style="width: 100%"
										:prop="`variablePropertys.extendFieldString9`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString9"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域10" style="width: 100%"
										:prop="`variablePropertys.extendFieldString10`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString10"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-divider />



								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域11" style="width: 100%"
										:prop="`variablePropertys.extendFieldString11`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString11"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域12" style="width: 100%"
										:prop="`variablePropertys.extendFieldString12`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString12"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域13" style="width: 100%"
										:prop="`variablePropertys.extendFieldString13`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString13"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域14" style="width: 100%"
										:prop="`variablePropertys.extendFieldString14`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString14"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域15" style="width: 100%"
										:prop="`variablePropertys.extendFieldString15`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString15"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域16" style="width: 100%"
										:prop="`variablePropertys.extendFieldString16`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString16"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域17" style="width: 100%"
										:prop="`variablePropertys.extendFieldString17`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString17"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域18" style="width: 100%"
										:prop="`variablePropertys.extendFieldString18`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString18"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域19" style="width: 100%"
										:prop="`variablePropertys.extendFieldString19`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString19"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域20" style="width: 100%"
										:prop="`variablePropertys.extendFieldString20`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString20"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-divider />


								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域21" style="width: 100%"
										:prop="`variablePropertys.extendFieldString21`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString21"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域22" style="width: 100%"
										:prop="`variablePropertys.extendFieldString22`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString22"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域23" style="width: 100%"
										:prop="`variablePropertys.extendFieldString23`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString23"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域24" style="width: 100%"
										:prop="`variablePropertys.extendFieldString24`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString24"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域25" style="width: 100%"
										:prop="`variablePropertys.extendFieldString25`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString25"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域26" style="width: 100%"
										:prop="`variablePropertys.extendFieldString26`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString26"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域27" style="width: 100%"
										:prop="`variablePropertys.extendFieldString27`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString27"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域28" style="width: 100%"
										:prop="`variablePropertys.extendFieldString28`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString28"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域29" style="width: 100%"
										:prop="`variablePropertys.extendFieldString29`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString29"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
								<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
									<el-form-item label="扩展域30" style="width: 100%"
										:prop="`variablePropertys.extendFieldString30`">
										<el-input v-model="deviceVariableForm.variablePropertys.extendFieldString30"
											style="width: 100%" clearable />
									</el-form-item>
								</el-col>
							</template>
							<el-empty description="空" v-else></el-empty>
						</el-row>
					</el-form>
				</el-tab-pane>



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
import { DeviceApi, DeviceVariableApi } from '/@/api-services/api';

import { DeviceVariable, VariableAlarm } from '/@/api-services/models';
import { dataTypeList, protectTypeList } from '../variable';


export default defineComponent({
	name: 'deviceVariableEdit',
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
		const deviceVariableFormRef = ref();
		const state = reactive({
			loading: false,
			isShowDialog: false,
			deviceVariableForm: {} as DeviceVariable,

			deviceNameList: [{
				type: Array<any>,
				default: () => [],
			},],

			deviceMedsList: [{
				type: Array<any>,
				default: () => [],
			},],
			hisTypes: [
				{ value: 0, label: '每次变化' },
				{ value: 1, label: '每次采集' },
			],

		});

		onMounted(async () => {
			state.loading = true;
			let resDicData = await getAPI(DeviceApi).apiDeviceGetDeviceNamesGet();
			state.deviceNameList = resDicData.data.result as Array<any>;

			state.loading = false;

		});

		const onDeviceFocus = async () => {

			let resDicData = await getAPI(DeviceApi).apiDeviceGetDeviceNamesGet();
			state.deviceNameList = resDicData.data.result as Array<any>;

		};
		// 打开弹窗
		const openDialog = (row: any) => {
			state.deviceVariableForm = JSON.parse(JSON.stringify(row));
			onDeviceFocus();
			if (state.deviceVariableForm.id != undefined)
				deviceChange(state.deviceVariableForm.deviceId)

			if (state.deviceVariableForm.name == undefined)
				state.deviceVariableForm.name = "var";
			if (state.deviceVariableForm.invokeInterval == undefined)
				state.deviceVariableForm.invokeInterval = 1000;
			if (state.deviceVariableForm.protectType == undefined)
				state.deviceVariableForm.protectType = 1;
			if (state.deviceVariableForm.coreDataType == undefined)
				state.deviceVariableForm.coreDataType = 7;

			state.isShowDialog = true;
		};
		// 关闭弹窗
		const closeDialog = () => {
			mittBus.emit('deviceVariableRefresh');
			state.isShowDialog = false;
		};
		// 取消
		const cancel = () => {
			state.isShowDialog = false;
		};

		const deviceChange = async (name: any) => {
			let resDicData = await getAPI(DeviceApi).apiDeviceGetDeviceMethodsGet(name);
			state.deviceMedsList = resDicData.data.result as Array<any>;
		};
		// 提交
		const submit = () => {

			deviceVariableFormRef.value.validate(async (isValid: boolean) => {
				if (!isValid) return;
				if (state.deviceVariableForm.id != undefined && state.deviceVariableForm.id > 0) {
					let a = getAPI(DeviceVariableApi)
					let b = state.deviceVariableForm
					let c = await a.apiDeviceVariableUpdateVariableConfigPost(b)
					// await getAPI(DeviceVariableApi).collectDeviceVariableUpdatePost(state.deviceVariableForm);
				} else {
					await getAPI(DeviceVariableApi).apiDeviceVariableAddDeviceVariablePost(state.deviceVariableForm);
				}
				closeDialog();
			});
		};

		const reAlarm = async (row: any) => {
			if (JSON.stringify(row) !== '{}') {
				var resRole = await getAPI(DeviceVariableApi).apiDeviceVariableAddVariableAlarmPost(row);
				state.deviceVariableForm.variableAlarms = resRole.data.result;
			}
		};

		const reHis = async (row: any) => {
			if (JSON.stringify(row) !== '{}') {
				var resRole = await getAPI(DeviceVariableApi).apiDeviceVariableAddVariableHisPost(row);
				state.deviceVariableForm.variableHiss = resRole.data.result;
			}
		};

		const reEx = async (row: any) => {
			if (JSON.stringify(row) !== '{}') {
				var resRole = await getAPI(DeviceVariableApi).apiDeviceVariableExportFilePost(row);
				state.deviceVariableForm.variablePropertys = resRole.data.result;
			}
		};

		return {
			deviceVariableFormRef,
			openDialog,
			closeDialog,
			cancel,
			reAlarm,
			reHis, reEx,
			onDeviceFocus, deviceChange,
			dataTypeList, protectTypeList,
			submit,
			...toRefs(state),
		};
	},
});
</script>
