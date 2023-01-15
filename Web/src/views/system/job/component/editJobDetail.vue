<template>
	<div class="sys-jobDetail-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="1000px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-tabs>
				<el-tab-pane label="作业信息">
					<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="110px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="作业编号" prop="jobId" :rules="[{ required: true, message: '作业编号不能为空', trigger: 'blur' }]">
									<el-input v-model="ruleForm.jobId" placeholder="作业编号" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="组名称" prop="groupName" :rules="[{ required: true, message: '组名称不能为空', trigger: 'blur' }]">
									<el-input v-model="ruleForm.groupName" placeholder="组名称" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="执行方式" prop="concurrent">
									<el-radio-group v-model="ruleForm.concurrent">
										<el-radio :label="true">并行</el-radio>
										<el-radio :label="false">串行</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="扫描特性触发器" prop="includeAnnotations">
									<el-radio-group v-model="ruleForm.includeAnnotations">
										<el-radio :label="true">是</el-radio>
										<el-radio :label="false">否</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="额外数据" prop="properties">
									<el-input v-model="ruleForm.properties" placeholder="额外数据" clearable type="textarea" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="描述信息" prop="description">
									<el-input v-model="ruleForm.description" placeholder="描述信息" clearable type="textarea" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="脚本代码">
					<div ref="monacoEditorRef" style="width: 100%; height: 500px"></div>
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
import { reactive, toRefs, defineComponent, ref } from 'vue';
import mittBus from '/@/utils/mitt';
import * as monaco from 'monaco-editor';
import { JobScriptCode } from './JobScriptCode';

import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { UpdateJobDetailInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditJobDetail',
	components: {},
	props: {
		title: {
			type: String,
			default: '',
		},
	},
	setup() {
		const ruleFormRef = ref();
		const monacoEditorRef = ref();
		const state = reactive({
			isShowDialog: false,
			ruleForm: {} as UpdateJobDetailInput,
			monacoEditor: null as any,
		});
		// 初始化monacoEditor对象
		var monacoEditor: any = null;
		const initMonacoEditor = () => {
			monacoEditor = monaco.editor.create(monacoEditorRef.value, {
				theme: 'vs-dark', // 主题 vs vs-dark hc-black
				value: JobScriptCode, // 默认显示的值
				language: 'csharp',
				formatOnPaste: true,
				wordWrap: 'on', //自动换行，注意大小写
				wrappingIndent: 'indent',
				folding: true, // 是否折叠
				foldingHighlight: true, // 折叠等高线
				foldingStrategy: 'indentation', // 折叠方式  auto | indentation
				showFoldingControls: 'always', // 是否一直显示折叠 always | mouSEOver
				disableLayerHinting: true, // 等宽优化
				emptySelectionClipboard: false, // 空选择剪切板
				selectionClipboard: false, // 选择剪切板
				automaticLayout: true, // 自动布局
				codeLens: false, // 代码镜头
				scrollBeyondLastLine: false, // 滚动完最后一行后再滚动一屏幕
				colorDecorators: true, // 颜色装饰器
				accessibilitySupport: 'auto', // 辅助功能支持  "auto" | "off" | "on"
				lineNumbers: 'on', // 行号 取值： "on" | "off" | "relative" | "interval" | function
				lineNumbersMinChars: 5, // 行号最小字符   number
				//enableSplitViewResizing: false,
				readOnly: false, //是否只读  取值 true | false
			});
		};
		// 打开弹窗
		const openDialog = (row: any) => {
			state.ruleForm = JSON.parse(JSON.stringify(row));
			state.isShowDialog = true;

			// 延迟拿值防止取不到
			setTimeout(() => {
				if (monacoEditor == null) initMonacoEditor();
				else monacoEditor.setValue(row.id == undefined ? JobScriptCode : state.ruleForm.scriptCode);
			}, 1);
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
				state.ruleForm.scriptCode = monacoEditor.getValue();
				if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
					await getAPI(SysJobApi).apiSysJobUpdateJobDetailPost(state.ruleForm);
				} else {
					await getAPI(SysJobApi).apiSysJobAddJobDetailPost(state.ruleForm);
				}
				closeDialog();
			});
		};
		return {
			ruleFormRef,
			monacoEditorRef,
			openDialog,
			closeDialog,
			cancel,
			submit,
			...toRefs(state),
		};
	},
});
</script>
