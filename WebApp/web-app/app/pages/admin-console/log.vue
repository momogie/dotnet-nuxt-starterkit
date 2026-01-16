<template>
  <div class="-mt-4">
    <div>
      <div class="border-b flex space-x-2 mb-4 -mx-4 px-4 my-1">
        <button
          v-for="tab in tabs"
          :key="tab.name"
          @click="goTo(tab.path)"
          :class="[
            'px-4 py-1 font-medium text-sm',
            $route.path === tab.path ? 'bg-primary/10 text-blue-500 border border-b-0 rounded-t ' : 'text-gray-600'
          ]"
        >
          {{ tab.label }}
        </button>
      </div>
    </div>
    <nuxt-page></nuxt-page>
  </div>
</template>

<script>
export default {
  data: () => ({
    selectedItem: {},
    tabs: [
      { name: 'profile', label: 'Data Changes', path: '/admin-console/log/data' },
      { name: 'profile', label: 'Errors', path: '/admin-console/log/error' },
    ]
  }),
  computed: {
    app: function() {
      return useApp();
    }
  },
  mounted: function() {
    this.app.setMenus([{Title: 'Admin Console'}, { Title: 'Logs'}])
  },
  methods: {
    goTo(path) {
      this.$router.push(path)
    },
    create: function() {
      return new Promise((resolve, reject) => {
        this.$modal.show('create')
        resolve();
      })
    },
    edit: function(v) {
        this.selectedItem = v;
      return new Promise((resolve, reject) => {
        this.$modal.show('edit')
        resolve();
      })
    },
    activate: function(v, y) {
      return new Promise((resolve, reject) => {
        this.$swal.confirm(y ? 'Activate User' : 'Deactivate User')
          .then(() => {
              this.$http.patch('/identity/user/activation?id='+ v, { IsActive: y})
                .then(() => {
                  useDataSource().load();
                  resolve();
                })
                .catch(err => {
                  if(err.response) {
                    this.$swal.error('Failed!', err.response?.data?.Message)
                  }
                  reject()
                })
          })
          .catch(_ => resolve())
      })
    },
    remove: function(v) {
      return new Promise((resolve, reject) => {
        this.$swal.confirmDelete()
          .then(() => {
              this.$http.delete('/identity/user/remove?id='+ v)
                .then(() => {
                  useDataSource().load();
                  resolve();
                })
                .catch(err => {
                  if(err.response) {
                    this.$swal.error('Failed!', err.response?.data)
                  }
                  reject()
                })
          })
          .catch(_ => resolve())
      })
    },
  }
}
</script>

<style scoped>
button:focus {
  outline: none;
}
</style>