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
      <Permission class="-mx-4 mt-5" 
        v-if="isLoaded"
        v-model="model.Claims"
      />
    </form>
  </b-modal>
</template>

<script>
import Permission from './permission-list.x.vue'
export default {
  components: { Permission },
  props: ['data'],
  data: () => ({
    isLoading: false,
    isLoaded: false,
    model: {
      Name: null,
      Claims: [],
    },
    errors: {},
  }),
  mounted: function () {
  },
  methods: {
    load: function() {
      this.isLoaded = false;
      this.$http.get('/identity/role/detail?id=' + this.data.Id)
        .then(({data}) => {
          this.model = data.Data;
          this.isLoaded = true;
        })
    },
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
      this.load();
    },
    hidden: function () {
      this.model = {
        Name: null,
        Claims: [],
      };
      this.isLoaded = false;
      this.errors = {};
    }
  }
}
</script>