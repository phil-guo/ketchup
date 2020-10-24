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

    <el-dialog :title="title" :visible.sync="dialogFormVisible" width="600px">
      <el-row>
        <el-col :span="20">
          <el-form :model="operateItem" :rules="operateAddOrUpdateRules" ref="operateItem">
            <el-form-item label="名称" label-width="120px" prop="name">
              <el-input v-model="operateItem.name"></el-input>
            </el-form-item>
            <el-form-item label="地址" label-width="120px" prop="url">
              <el-input v-model="operateItem.url" width="120px"></el-input>
            </el-form-item>
            <el-form-item label="图标" label-width="120px">
              <el-input v-model="operateItem.icon" width="120px"></el-input>
            </el-form-item>
            <el-form-item label="层级" label-width="120px">
              <el-select v-model="operateItem.level" clearable placeholder="菜单级别" width="120px">
                <el-option v-for="item in leavelDatas" :key="item.value" :label="item.key" :value="parseInt(item.value)"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="父级菜单" label-width="120px" v-if="operateItem.level==2">
              <el-select v-model="operateItem.parentId" clearable placeholder="请选择父级菜单" width="120px">
                <el-option v-for="item in parentMenuDatas" :key="item.id" :label="item.name" :value="parseInt(item.id)"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item label="菜单按钮" label-width="120px" v-if="operateItem.level==2">
              <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">{{ "全选" }}</el-checkbox>
              <el-checkbox-group v-model="operateItem.operateArray" @change="handleCheckedCitiesChange">
                <el-checkbox v-for="item in permissions" :label="item.id" :key="item.id">{{item.name}}</el-checkbox>
              </el-checkbox-group>
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
import $ from "jquery";
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
          title: "名称",
          key: "name"
        },
        {
          title: "图标",
          key: "icon"
        },
        {
          title: "级别",
          key: "level",
          formatter(row, column, cellValue, index) {
            if (cellValue == 1) {
              return "顶级";
            } else if (cellValue == 2) {
              return "一级";
            }
          }
        },
        {
          title: "路由",
          key: "url"
        },
        {
          title: "功能",
          key: "operateModels",
          formatter(row, column, cellValue, index) {
            var displayStr = "";
            $.each(cellValue, function(index, element) {
              displayStr += element.name + ",";
            });
            return displayStr;
          }
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

      //父级菜单
      parentMenuDatas: [],
      leavelDatas: [
        { key: "顶级", value: 1 },
        { key: "一级", value: 2 }
      ],

      //表单
      checkAll: false,
      isIndeterminate: false,
      permissions: [],
      dialogFormVisible: false,
      title: "",
      operateItem: {
        id: 0,
        name: "",
        url: "",
        level: "",
        parentId: "",
        icon: "folder-o",
        operates: "[]",
        operateArray: [],
        operatesDictionary: [],
        OperateModels: []
      },
      operateAddOrUpdateRules: {
        name: [
          {
            required: true,
            message: "请输入菜单名称",
            trigger: "blur"
          }
        ],
        url: [
          {
            required: true,
            message: "请输入菜单路径",
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
    handleCheckedCitiesChange(value) {
      let vm = this;
      let checkedCount = value.length;
      vm.checkAll = checkedCount === vm.permissions.length;
      vm.isIndeterminate =
        checkedCount > 0 && checkedCount < vm.permissions.length;
    },
    handleCheckAllChange(val) {
      let vm = this;
      if (val) {
        $.each(vm.permissions, (key, item) => {
          vm.operateItem.operateArray.push(item.id);
        });
        vm.isIndeterminate = true;
      } else {
        vm.operateItem.operateArray = [];
        vm.isIndeterminate = false;
      }
    },
    getParentMenus() {
      let vm = this;
      var params = {
        pageIndex: 1,
        pageMax: 200,
        parentId: 99999
      };
      util.http.post(util.requestUrl.pageSearchMenu, params, vm, function(
        response
      ) {
        vm.parentMenuDatas = response.datas;
      });
    },
    getOperates() {
      let vm = this;
      var params = {
        pageIndex: 1,
        pageMax: 1000
      };
      util.http.post(util.requestUrl.pageSearchOperate, params, vm, function(
        response
      ) {
        vm.permissions = response.datas;
      });
    },
    search() {},
    addOrEditSubmit(form) {
      let vm = this;
      vm.$refs[form].validate(valid => {
        if (!valid) {
          return false;
        }
        if (vm.operateItem.level == 0) {
          vm.operateItem.parentId = 99999;
        } else {
          vm.operateItem.parentId =
            vm.operateItem.parentId == "" ? 99999 : vm.operateItem.parentId;
        }
        if (vm.operateItem.operateArray.length > 0) {
          vm.operateItem.operates = JSON.stringify(vm.operateItem.operateArray);
        }
        util.http.post(
          util.requestUrl.menuAddOrEdit,
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
    addOrEditRow(type) {
      let vm = this;
      vm.getOperates();
      vm.getParentMenus();
      if (type === "add") {
        vm.title = "添加";
        vm.operateItem.id = 0;
        vm.operateItem.name = "";
        vm.operateItem.url = "";
        vm.operateItem.level = "";
        vm.operateItem.parentId = "";
        vm.operateItem.icon = "folder-o";
        vm.dialogFormVisible = true;
        vm.operateItem.operateArray = [];
      }
      if (type === "edit") {
        if (vm.multipleSelection == null || vm.multipleSelection.length != 1) {
          this.$notify.error({
            title: globalSetting.operateErrorMsg,
            message: "请选取一行数据操作"
          });
        } else {
          let row = vm.multipleSelection[0];
          vm.title = "编辑";
          vm.operateItem.id = row.id;
          vm.operateItem.name = row.name;
          vm.operateItem.url = row.url;
          vm.operateItem.level = row.level;
          vm.operateItem.parentId = row.parentId;
          vm.operateItem.icon = row.icon;
          if (row.operates.length > 0) {
            vm.operateItem.operateArray = JSON.parse(row.operates);
          }
          vm.dialogFormVisible = true;
        }
      }
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
              util.requestUrl.removeMenu,
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
    pageSearch(pageCurrent) {
      let vm = this;
      vm.params.pageIndex = pageCurrent;
      vm.params.pageMax = vm.pagination.pageSize;

      util.http.post(util.requestUrl.pageSearchMenu, vm.params, vm, function(
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