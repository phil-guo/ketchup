<template>
  <d2-container>
    <d2-crud ref="d2Crud" :columns="columns" :data="data">
      <!-- <ZeroComponent
        slot="header"
        style="margin-bottom: 5px"
        @zero-look="entryLook"
      /> -->
    </d2-crud>
  </d2-container>
</template>

<script>
import ZeroComponent from "@/views/permissions/zero.component/index.vue";
import Cluster from "@/views/server/entry/components/cluster";
import Services from "@/views/server/entry/components/services";
import util from "@/libs/util.js";
export default {
  components: {
    ZeroComponent,
    Cluster,
    Services,
  },
  data() {
    return {
      columns: [
        {
          title: "服务名",
          key: "name",
          width: "180",
        },
        {
          title: "集群条目",
          key: "cluster",
          component: {
            name: Cluster,
          },
        },
        {
          title: "服务",
          key: "services",
          width: "300",
          component: {
            name: Services,
          },
        },
      ],
      data: [],
    };
  },
  created() {
    this.getData();
  },
  methods: {
    getData() {
      let vm = this;
      util.http.get(util.requestUrl.getAllServer, vm, function (response) {
        vm.data = response;
      });
    },
    entryLook() {},
  },
};
</script>

<style scoped>
</style>