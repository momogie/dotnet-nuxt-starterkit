
var app = useNuxtApp();

export const usePreference = defineStore('Preference', {
  state: () => ({
    isLoading: false,
    isEditing: false,
    isServerError: false,
    isNetworkError: false,
    data: [],
  }),
  actions: {
    load: function() {
      this.isLoading = true;
      this.isNetworkError = this.isServerError = false;
      return new Promise((resolve, reject) => {
        app.$http.get(`/identity/preference/list`)
          .then(({data}) => {
            this.data = data.Data;
            resolve(data);
          })
          .catch(err => {
            if(err.code == 'ERR_NETWORK')
              this.isNetworkError = true;
            
            if(err.code == 'ERR_BAD_RESPONSE')
              this.isServerError = true;

            reject(err);
          })
          .finally(_ => this.isLoading = false);
      })
    },
    setPage: function(v) {
      this.filter.Page = v;
      this.load();
    },
    setLength: function(v) {
      this.filter.Length = v;
      this.load();
    },
    edit: function(key, val) {
      this.isEditing = true;
      return new Promise((resolve, reject) => {
        app.$http.post(`/identity/preference/edit`, { Key: key, Value: JSON.stringify(val)})
          .then(({data}) => {
            var idx = this.data.findIndex(p => p.Key == key)
            this.data[idx] = data.Data;
            resolve(data);
          })
          .catch((err) => reject(err.response?.data))
          .finally(_ => this.isEditing = false);
      })
    }
  },
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(usePreference, import.meta.hot));
}