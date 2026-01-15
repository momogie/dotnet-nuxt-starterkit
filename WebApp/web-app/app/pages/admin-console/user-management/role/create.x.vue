<template>
  <b-modal 
    id="create" 
    title="Add New Role" 
    @hidden="hidden"
    @submit="submit" 
    :is-loading="isLoading"
  >
    <form>
      <input-text label="Name *" v-model="model.Name" :errors="errors?.Name" />
    </form>
  </b-modal>
</template>

<script>
export default {
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