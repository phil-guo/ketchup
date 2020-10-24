<template>
  <d2-container>
    <el-row :gutter="5">
      <el-col :span="3">
        <el-input v-model="name" clearable placeholder="请输入名称" style="margin-bottom: 5px"></el-input>
      </el-col>
    </el-row>
    <d2-crud :columns="columns" :data="data" :loading="loading" selection-row @selection-change="handleSelectionChange" :pagination="pagination"
      @pagination-current-change="paginationCurrentChange">
      <zero-permission slot="header" style="margin-bottom: 5px" @zero-addEdit="addOrEditRow" @zero-search="search" @zero-remove="remove">
      </zero-permission>
    </d2-crud>
    <!--添加编辑弹窗-->
    <el-dialog :title="title" :visible.sync="dialogFormVisible" width="600px">
      <el-row>
        <el-col :span="20">
          <el-form :model="operateItem" :rules="operateAddOrEditRules" ref="operateItem">
            <el-form-item label="用户名" label-width="120px" prop="userName">
              <el-input v-model="operateItem.userName"></el-input>
            </el-form-item>
            <el-form-item label="角色" label-width="120px" prop="roleId">
              <el-select v-model="operateItem.roleId" clearable placeholder="请选择角色" width="120px">
                <el-option v-for="item in roles" :key="item.id" :label="item.name" :value="parseInt(item.id)"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item v-if="formType==false" label="密码" label-width="120px" prop="password">
              <el-input v-model="operateItem.password" show-password></el-input>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="addOrEditSubmit('operateItem')">确 定</el-button>
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

      //table
      data: [],
      columns: [
        {
          title: "用户",
          key: "userName"
        },
        {
          title: "角色",
          key: "roleName"
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
      roles: [],
      formType: false,
      title: "",
      operateItem: {
        id: 0,
        userName: "",
        password: "",
        roleId: 0
      },
      dialogFormVisible: false,
      operateAddOrEditRules: {
        userName: [
          {
            required: true,
            message: "请输入用户名",
            trigger: "blur"
          }
        ],
        password: [
          {
            required: true,
            message: "请输入用户密码",
            trigger: "blur"
          }
        ]
      }
    };
  },
  mounted() {
    let vm = this;
    vm.pageSearch(vm.pagination.currentPage);
  },
  methods: {
    getRoles() {
      let vm = this;
      var params = {
        pageIndex: 1,
        pageMax: 1000,
      };
      util.http.post(util.requestUrl.pageSearchRole, params, vm, function(
        response
      ) {
        vm.roles = response.datas;
      });
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
              util.requestUrl.removeSysUser,
              { id: row.id },
              vm,
              function(response) {
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
    search() {
      let vm = this;
      vm.pagination.currentPage = 1;
      if (vm.name != "" || vm.name != null) {
        vm.params.name = vm.name;
      }
      vm.pageSearch(vm.pagination.currentPage);
    },
    addOrEditRow(type) {
      let vm = this;
      vm.getRoles();
      if (type === "add") {
        vm.title = "添加";
        vm.operateItem.id = 0;
        vm.operateItem.name = "";
        vm.operateItem.password = "";
        vm.formType = false;
        vm.operateItem.roleId = 1;
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
          vm.operateItem.userName = row.userName;
          vm.operateItem.roleId = row.roleId;
          vm.formType = true;
          vm.dialogFormVisible = true;
        }
      }
    },
    addOrEditSubmit(form) {
      let vm = this;
      vm.$refs[form].validate(valid => {
        util.http.post(
          util.requestUrl.createOrEditSysUser,
          vm.operateItem,
          vm,
          function(response) {
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
    pageSearch(pageCurrent) {
      let vm = this;
      vm.params.pageIndex = pageCurrent;
      vm.params.pageMax = vm.pagination.pageSize;

      util.http.post(util.requestUrl.pageSerachSysUser, vm.params, vm, function(
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
};
</script>