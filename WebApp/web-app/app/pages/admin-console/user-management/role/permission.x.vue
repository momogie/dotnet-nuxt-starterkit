<template>
  <b-modal 
    id="permission" 
    title="Add New Role" 
    @hidden="hidden"
    @submit="submit" 
    :is-loading="isLoading"
    size="w-5/12"
  >
    <form>
      <input-text label="Name *" v-model="model.Name" :errors="errors?.Name" />
      <Test class="-mx-4 ms-0 mt-5" />
    </form>
  </b-modal>
</template>

<script>
import Test from './test.vue'
export default {
  components: {Test },
  data: () => ({
    isLoading: false,
    model: {
      Name: null,
    },
    errors: {},
  }),
  mounted: function () {
  },
  methods: {
    submit: async function () {
      this.isLoading = true;
      this.$http.post('/identity/role/create', this.model)
        .then(({data}) => {
          useDataSource().load();
          this.$modal.hide('create')
        })
        .catch(err => {
          this.errors = err.response?.data?.Errors || {}
        })
        .finally(_ => this.isLoading = false)
    },
    hidden: function () {
      this.model = {
        Name: null,
      };
      this.errors = {};
    }
  }
}
</script>