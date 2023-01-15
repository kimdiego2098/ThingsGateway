<template>
	<el-card class="box-card" shadow="hover" style="height: 100%" body-style="height:100%; overflow:auto">
		<template #header>
			<div class="card-header">
				<div class="tree-h-flex">
					<div class="tree-h-left">
						<el-input :prefix-icon="Search" v-model="filterText" placeholder="机构名称" />
					</div>
					<div class="tree-h-right">
						<el-dropdown @command="handleCommand">
							<el-button style="margin-left: 8px; width: 34px">
								<el-icon class="el-icon--center">
									<more-filled />
								</el-icon>
							</el-button>
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item command="expandAll">全部展开</el-dropdown-item>
									<el-dropdown-item command="collapseAll">全部折叠</el-dropdown-item>
									<el-dropdown-item command="rootNode">根节点</el-dropdown-item>
									<el-dropdown-item command="refresh">刷新</el-dropdown-item>
								</el-dropdown-menu>
							</template>
						</el-dropdown>
					</div>
				</div>
			</div>
		</template>
		<div style="margin-bottom: 45px" v-loading="state.loading">
			<el-tree
				ref="treeRef"
				class="filter-tree"
				:data="state.orgData"
				node-key="id"
				:props="{ children: 'children', label: 'name' }"
				:filter-node-method="filterNode"
				@node-click="nodeClick"
				:show-checkbox="state.isShowCheckbox"
				:default-checked-keys="state.ownOrgData"
				highlight-current
				check-strictly
			/>
		</div>
	</el-card>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref, watch } from 'vue';
import type { ElTree } from 'element-plus';
import { Search, MoreFilled } from '@element-plus/icons-vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysOrgApi } from '/@/api-services/api';
import { SysOrg } from '/@/api-services/models';

const filterText = ref('');
const treeRef = ref<InstanceType<typeof ElTree>>();

const state = reactive({
	loading: false,
	orgData: [] as Array<SysOrg>,
	isShowCheckbox: false,
	ownOrgData: [],
});

onMounted(() => {
	initTreeData();
});

watch(filterText, (val) => {
	treeRef.value!.filter(val);
});

const initTreeData = async () => {
	state.loading = true;
	var res = await getAPI(SysOrgApi).apiSysOrgGetOrgListGet(0);
	state.orgData = res.data.result ?? [];
	state.loading = false;
};

// 设置默认选择
const setCheckedKeys = (orgData: any) => {
	state.isShowCheckbox = true;
	treeRef.value!.setCheckedKeys([]);
	state.ownOrgData = orgData;
};

// 获取已经选择
const getCheckedKeys = () => {
	return treeRef.value!.getCheckedKeys();
};

const filterNode = (value: string, data: any) => {
	if (!value) return true;
	return data.name.includes(value);
};

const handleCommand = async (command: string | number | object) => {
	if ('expandAll' == command) {
		for (let i = 0; i < treeRef.value!.store._getAllNodes().length; i++) {
			treeRef.value!.store._getAllNodes()[i].expanded = true;
		}
	} else if ('collapseAll' == command) {
		for (let i = 0; i < treeRef.value!.store._getAllNodes().length; i++) {
			treeRef.value!.store._getAllNodes()[i].expanded = false;
		}
	} else if ('refresh' == command) {
		initTreeData();
	} else if ('rootNode' == command) {
		emits('node-click', { id: 0, name: '' });
	}
};

// 与父组件的交互逻辑
const emits = defineEmits(['node-click']);
const nodeClick = (node: any) => {
	emits('node-click', { id: node.id, name: node.name });
};

//const orgTreeData = state.orgData; // 异步数据导出不了？
// 导出
defineExpose({ initTreeData, setCheckedKeys, getCheckedKeys });
</script>

<style scoped>
.tree-h-flex {
	display: flex;
}

.tree-h-left {
	flex: 1;
	width: 100%;
}

.tree-h-right {
	width: 42px;
	min-width: 42px;
}
</style>
