<template>
  <d2-container>
    <el-row :gutter="5">
      <el-col :span="3">
        <el-input v-model="name" clearable placeholder="请输入名称" style="margin-bottom: 5px"></el-input>
      </el-col>
    </el-row>
    <d2-crud :columns="columns" :data="data" :loading="loading" selection-row @selection-change="handleSelectionChange"
      :pagination="pagination" @pagination-current-change="paginationCurrentChange">
      <zero-permission slot="header" style="margin-bottom: 5px" @zero-addEdit="addOrEditRow" @zero-search="search"
        @zero-remove="remove" @zero-permission="permission">
      </zero-permission>
    </d2-crud>
    <!--添加编辑弹窗-->
    <el-dialog :title="title" :visible.sync="dialogFormVisible" width="600px">
      <el-row>
        <el-col :span="20">
          <el-form :model="operateItem" :rules="operateAddOrUpdateRules" ref="operateItem">
            <el-form-item label="名称" label-width="120px" prop="name">
              <el-input v-model="operateItem.name"></el-input>
            </el-form-item>
            <el-form-item label="备注" label-width="120px">
              <el-input v-model="operateItem.remark"></el-input>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="addOrEditSubmit('operateItem')">确 定</el-button>
      </div>
    </el-dialog>

    <!--设置权限弹窗树结构-->
    <el-dialog :title="title" :visible.sync="dialogTreeVisible" width="600px">
      <el-row>
        <el-col :span="12" :offset="7">
          <el-tree :data="treeData" show-checkbox node-key="id" ref="tree" :default-checked-keys="keys"
            :props="defaultProps">
          </el-tree>
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogTreeVisible = false">取 消</el-button>
        <el-button type="primary" @click="sumibitPermission">确 定</el-button>
      </div>
    </el-dialog>

  </d2-container>
</template>
<script>
  import zeroComponent from "@/views/permissions/zero.component/index.vue";
  import util from "@/libs/util.js";
  export default {
    components: {
      "zero-permission": zeroComponent
    },
    data() {
      return {
        //查询条件
        name: "",
        params: {},

        //tree
        dialogTreeVisible: false,
        treeData: [],
        keys: [],
        defaultProps: {
          children: "children",
          label: "lable"
        },

        //table
        data: [],
        columns: [{
            title: "角色名称",
            key: "name"
          },
          {
            title: "备注",
            key: "remark"
          }
        ],
        loading: true,

        //分页
        pagination: {
          currentPage: 1,
          pageSize: 20,
          total: 0
        },
        // checkbox选择
        multipleSelection: [],

        //表单
        roleId: 0,
        title: "",
        operateItem: {
          id: 0,
          name: "",
          remark: ""
        },
        dialogFormVisible: false,
        operateAddOrUpdateRules: {
          name: [{
            required: true,
            message: "请输入角色名称",
            trigger: "blur"
          }]
        }
      }
    },
    mounted() {
      let vm = this;
      vm.pageSearch(vm.pagination.currentPage);
    },
    methods: {
      permission() {
        let vm = this;
        if (vm.multipleSelection == null || vm.multipleSelection.length != 1) {
          vm.$notify.error({
            title: util.globalSetting.operateErrorMsg,
            message: "请选取一行数据操作"
          });
        } else {
          let row = vm.multipleSelection[0];
          vm.title = "设置权限";
          vm.roleId = row.id;
          vm.getMenusSetRole(row.id);
        }
      },
      getMenusSetRole(roleId) {
        let vm = this;
        util.http.post(
          util.requestUrl.getMenusSetRole, {
            roleId: roleId
          },
          vm,
          function (response) {
            vm.treeData = response.list;
            vm.keys = response.menuIds;
            vm.dialogTreeVisible = true;
          }
        );
      },
      search() {
        let vm = this;
        vm.pagination.currentPage = 1;
        if (vm.name != "" || vm.name != null) {
          vm.params.name = vm.name;
        }
        vm.pageSearch(vm.pagination.currentPage);
      },
      remove() {
        let vm = this;
        if (vm.multipleSelection == null || vm.multipleSelection.length != 1) {
          this.$notify.error({
            title: util.globalSetting.operateErrorMsg,
            message: "请选取一行数据操作"
          });
        } else {
          this.$confirm("此操作将永久删除该记录, 是否继续?", "提示", {
              confirmButtonText: "确定",
              cancelButtonText: "取消",
              type: "warning",
              center: true
            })
            .then(() => {
              let row = vm.multipleSelection[0];
              util.http.post(
                util.requestUrl.removeRole, {
                  id: row.id
                },
                vm,
                function (response) {
                  vm.$notify({
                    title: "成功",
                    duration: 3000,
                    message: util.globalSetting.operateSuccessMsg,
                    type: "success"
                  });
                  vm.dialogFormVisible = false;
                  vm.pageSearch(vm.pagination.currentPage);
                }
              );
            })
            .catch(() => {
              this.$message({
                type: "info",
                message: "已取消删除"
              });
            });
        }
      },
      addOrEditRow(type) {
        let vm = this;
        if (type === "add") {
          vm.title = "添加";
          vm.operateItem.id = 0;
          vm.operateItem.name = "";
          vm.operateItem.remark = "";
          vm.dialogFormVisible = true;
        }
        if (type === "edit") {
          if (vm.multipleSelection == null || vm.multipleSelection.length != 1) {
            this.$notify.error({
              title: util.globalSetting.operateErrorMsg,
              message: "请选取一行数据操作"
            });
          } else {
            let row = vm.multipleSelection[0];
            vm.title = "编辑";
            vm.operateItem.id = row.id;
            vm.operateItem.name = row.name;
            vm.operateItem.remark = row.remark;
            vm.dialogFormVisible = true;
          }
        }
      },
      addOrEditSubmit(form) {
        let vm = this;
        vm.$refs[form].validate(valid => {
          util.http.post(
            util.requestUrl.createOrEditRole,
            vm.operateItem,
            vm,
            function (response) {
              vm.$notify({
                title: "成功",
                duration: 3000,
                message: util.globalSetting.operateSuccessMsg,
                type: "success"
              });
              vm.dialogFormVisible = false;
              vm.pageSearch(vm.pagination.currentPage);
            }
          );
        });
      },
      sumibitPermission() {
        let vm = this;
        var params = {
          menuIds: vm.$refs.tree
            .getCheckedKeys()
            .concat(vm.$refs.tree.getHalfCheckedKeys()),
          roleId: vm.roleId
        };
        util.http.post(
          util.requestUrl.setRolePermission, params,
          vm,
          function (response) {
            vm.$notify({
              title: "成功",
              duration: 3000,
              message: util.globalSetting.operateSuccessMsg,
              type: "success"
            });
            vm.dialogTreeVisible = false;
          }
        );
      },
      pageSearch(pageCurrent) {
        let vm = this;
        vm.params.pageIndex = pageCurrent;
        vm.params.pageMax = vm.pagination.pageSize;

        util.http.post(util.requestUrl.pageSearchRole, vm.params, vm, function (
          response
        ) {
          vm.data = response.datas;
          vm.pagination.total = response.total;
          vm.loading = false;
        });
      },
      paginationCurrentChange(currentPage) {
        let vm = this;
        vm.pagination.currentPage = currentPage;
        vm.pageSearch(currentPage);
      },
      handleSelectionChange(selection) {
        this.multipleSelection = selection;
      }
    }
  }

</script>
