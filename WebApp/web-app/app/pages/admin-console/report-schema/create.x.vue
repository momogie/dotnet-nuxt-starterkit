<template>
  <b-modal 
    id="create" 
    title="Add Report Template" 
    @hidden="hidden"
    @submit="submit" 
    :is-loading="isLoading"
  >
    <form>
      <input-combobox 
        label="Data Source *" v-model="model.Name" :errors="errors?.Name" 
      />
      <input-text label="Data Source *" v-model="model.Name" :errors="errors?.Name" />
      <input-text label="Description *" v-model="model.Name" :errors="errors?.Name" 
        multiline
        rows="3"
      />
      <input-attachment label="Template File *" v-model="model.Name" :errors="errors?.Name" />
    </form>
  </b-modal>
</template>

<script>
export default {
  data: () => ({
    isLoading: false,
    model: {
      Name: null,
      Claims: [],
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