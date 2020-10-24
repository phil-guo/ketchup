<template>
  <div>
    <el-button v-if="isSearch" style="margin-right:-5px" v-show="isSearch" size="small" @click="search">查询</el-button>
    <el-button v-if="isInsert" style="margin-right:-5px" v-show="isInsert" size="small" @click="addEdit('add')">添加
    </el-button>
    <el-button v-if="isEdit" style="margin-right:-5px" v-show="isEdit" size="small" @click="addEdit('edit')">编辑
    </el-button>
    <el-button v-if="isDelete" style="margin-right:-5px" v-show="isDelete" size="small" @click="remove">删除</el-button>
    <el-button v-if="isPermission" style="margin-right:-5px" v-show="isPermission" size="small" @click="permission">权限
    </el-button>
  </div>
</template>
<script>
  import util from "@/libs/util.js";
  import $ from "jquery";
  import {
    fa
  } from 'faker/lib/locales';
  export default {
    data() {
      return {
        permissions: [],
        isInsert: false,
        isEdit: false,
        isSearch: false,
        isDelete: false,
        isPermission: false
      };
    },

    mounted() {
      this.getMenuOperate();
    },
    methods: {
      search() {
        this.$emit("zero-search");
      },
      addEdit(permissionType) {
        this.$emit("zero-addEdit", permissionType);
      },
      remove() {
        this.$emit("zero-remove");
      },
      permission() {
        this.$emit("zero-permission");
      },
      getMenuOperate() {
        let vm = this;
        var params = {
          roleId: parseInt(util.cookies.get(util.globalSetting.roleId)),
          menuId: parseInt(vm.$route.query.id)
        };

        util.http.post(util.requestUrl.getMenuOfOperate, params, vm, function (
          response
        ) {
          vm.permissions = response.datas;
          vm.showPermission();
        });
      },

      showPermission() {
        let vm = this;
        $.each(vm.permissions, (key, item) => {
          if (item == "1001") {
            vm.isInsert = true;
          } else if (item == "1002") {
            vm.isEdit = true;
          } else if (item == "1003") {
            vm.isSearch = true;
          } else if (item == "1004") {
            vm.isDelete = true;
          } else if (item == "1005") {
            vm.isPermission = true;
          }
        });
      }
    }
  };

</script>
