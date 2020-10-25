<template>
  <d2-container>
    <d2-crud
      ref="d2Crud"
      :columns="columns"
      :data="data"
      @d2-data-change="handleDataChange"
      selection-row
    >
      <ZeroComponent
        slot="header"
        style="margin-bottom: 5px"
        @zero-look="entryLook"
      />
    </d2-crud>
  </d2-container>
</template>

<script>
import ZeroComponent from "@/views/permissions/zero.component/index.vue";
import cluster from "@/views/server/entry/components/cluster";
import util from "@/libs/util.js";
export default {
  components: {
    ZeroComponent,
    cluster,
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
            name: cluster,
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
      util.http.post(util.requestUrl.getAllServer, {}, vm, function (response) {
        vm.data = response;
      });
    },
    entryLook() {},
    handleDataChange({ index, row }) {
      console.log(index);
      console.log(row);
    },
  },
};
</script>

<style scoped>

</style>