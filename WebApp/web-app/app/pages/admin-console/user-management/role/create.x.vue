<template>
  <b-modal 
    id="create" 
    title="Add New Role" 
    @hidden="hidden"
    @submit="submit" 
    :is-loading="isLoading"
  >
    <form>
      <!-- <pre>{{ model }}</pre> -->
      <input-text label="Name *" v-model="model.Name" :errors="errors?.Name" />
      <Permission class="-mx-4 mt-5" 
        v-model="model.Claims"
      />
    </form>
  </b-modal>
</template>

<script>
import Permission from './permission-list.x.vue'
export default {
  components: {Permission },
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