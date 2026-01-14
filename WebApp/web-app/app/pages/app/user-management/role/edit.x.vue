<template>
  <b-modal 
    id="edit" 
    title="Edit Role" 
    @hidden="hidden"
    @shown="shown"
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
  props: ['data'],
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
      this.$http.patch('/identity/role/edit?id='+this.data.Id, this.model)
        .then(({data}) => {
          useDataSource().load();
          this.$modal.hide('edit')
        })
        .catch(err => {
          this.errors = err.response?.data?.Errors || {}
        })
        .finally(_ => this.isLoading = false)
    },
    shown: function() {
      this.model = {...this.data || {}}
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