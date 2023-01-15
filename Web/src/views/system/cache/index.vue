<template>
	<div class="sys-cache-container">
		<NoticeBar text="系统缓存数据管理，请慎重操作！" leftIcon="iconfont icon-tongzhi2" background="var(--el-color-primary-light-9)" color="var(--el-color-primary)" />
		<el-row :gutter="8" style="width: 100%">
			<el-col :span="8" :xs="24">
				<el-card shadow="hover" header="缓存列表" v-loading="loading" class="mt8">
					<template #header>
						<div class="card-header">
							<span>缓存列表</span>
							<el-button icon="ele-Refresh" size="small" circle text @click="handleQuery" v-auth="'sysCache:keyList'" />
						</div>
					</template>
					<el-tree
						ref="treeRef"
						class="filter-tree"
						:data="cacheData"
						node-key="id"
						:props="{ children: 'children', label: 'name' }"
						@node-click="nodeClick"
						:default-checked-keys="cacheData"
						highlight-current
						check-strictly
						default-expand-all
						accordion
					/>
				</el-card>
			</el-col>
			<el-col :span="16" :xs="24">
				<el-card shadow="hover" header="缓存数据" v-loading="loading1" class="mt8">
					<template #header>
						<div class="card-header">
							<span>缓存数据</span>
							<el-button icon="ele-Delete" size="small" type="danger" @click="delCache" v-auth="'sysCache:delete'"> 删除缓存 </el-button>
						</div>
					</template>
					<vue-json-pretty :data="cacheValue" showLength showIcon showLineNumber showSelectController />
				</el-card>
			</el-col>
		</el-row>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, defineComponent, ref } from 'vue';
import { ElMessageBox, ElMessage, ElTree } from 'element-plus';
import NoticeBar from '/@/components/noticeBar/index.vue';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

import { getAPI } from '/@/utils/axios-utils';
import { SysCacheApi } from '/@/api-services';

export default defineComponent({
	name: 'sysCache',
	components: { NoticeBar, VueJsonPretty },
	setup() {
		const treeRef = ref<InstanceType<typeof ElTree>>();
		const currentNode = ref<any>({});
		const state = reactive({
			loading: false,
			loading1: false,
			cacheData: [] as any,
			cacheValue: undefined,
		});
		onMounted(async () => {
			handleQuery();
		});
		// 查询操作
		const handleQuery = async () => {
			state.cacheData = [];

			state.loading = true;
			var res = await getAPI(SysCacheApi).apiSysCacheGetCacheKeysGet();
			let keyList: any = res.data.result;

			// 构造树（以分号分割）
			for (let i = 0; i < keyList.length; i++) {
				let keyNames = keyList[i].split(':');
				let pName = keyNames[0];
				if (state.cacheData.filter((x: any) => x.name == pName).length === 0) {
					state.cacheData.push({
						id: 0,
						name: pName,
						children: [],
					});
				}
				if (keyNames.length > 1) {
					let pNode = state.cacheData.filter((x: any) => x.name == pName)[0] || {};
					pNode.children.push({
						id: keyList[i],
						name: keyList[i].substr(pName.length + 1),
					});
				}
			}
			state.loading = false;
		};
		// 删除
		const delCache = () => {
			if (currentNode.value.id == 0) {
				ElMessage.warning('禁止删除顶层缓存');
				return;
			}
			ElMessageBox.confirm(`确定删除缓存：【${currentNode.value.id}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysCacheApi).apiSysCacheRemoveKeyPost(currentNode.value.id);
					handleQuery();
					state.cacheValue = undefined;
					ElMessage.success('删除成功');
				})
				.catch(() => {});
		};
		// 树点击
		const nodeClick = async (node: any) => {
			if (node.id == 0) return;

			currentNode.value = node;
			state.loading1 = true;
			var res = await getAPI(SysCacheApi).apiSysCacheGetCacheValueKeyGet(node.id);
			state.cacheValue = res.data.result;
			state.loading1 = false;
		};
		return {
			treeRef,
			handleQuery,
			delCache,
			nodeClick,
			...toRefs(state),
		};
	},
});
</script>
+

<style lang="scss">
.card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
</style>
