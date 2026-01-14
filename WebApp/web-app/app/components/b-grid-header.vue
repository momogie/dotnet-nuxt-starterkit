<template>
  <th class="px-2 py-1 text-left truncate max-w-[320px] ellipsis-cell"
    ref="ths"
    :style="`width:${data.width}px`"  
  >
    <div class="cell-content">
      <div class="flex cursor-default">
        <div @click="() => toggleSort(data.PropertyName)">
          {{ data.Label }}
        </div>
        <div class="flex-1">
          <Icon v-if="sort.by == data.PropertyName && sort.direction == 'asc'" name="ph:caret-up-fill" size="18" />
          <Icon v-else-if="sort.by == data.PropertyName && sort.direction == 'desc'" name="ph:caret-down-fill"
            size="18" />
        </div>
        <div>
          <Popover v-if="data.filterable === true">
            <PopoverTrigger as-child>
              <Icon name="ph:sliders-horizontal-bold" size="16" class="ms-1 cursor-pointer" />
            </PopoverTrigger>
            <PopoverContent class="w-80">
              <div class="grid gap-4">
                <div class="space-y-2">
                  <h4 class="font-medium leading-none">
                    Filter {{ data.Label }}
                  </h4>
                </div>
                <div class="grid gap-2">
                  <div v-if="data.type === 'date'">
                    <input-date-range
                      :on-change="(v, a) => {
                        // var filters = ds.filter.Filters.filter(p =>  p[0] != data.PropertyName)
                        // if(v) {
                        //   filters.push([data.PropertyName, '>=', v]);
                        //   filters.push([data.PropertyName, '<=', a]);
                        // }
                        // setFilter(filters)
                      }"
                    />
                  </div>

                  <div v-if="data.type === 'datasource'" >
                    <input-dropdown :source="data.source" 
                      :filters="[]" 
                      :placeholder="`Select ${data.Label}`" 
                      value-field="Id"
                      text-field="Name"
                    />
                  </div>
                </div>
              </div>
            </PopoverContent>
          </Popover>
        </div>
        <div>
          <span class="resizer border-r" @mousedown="startResize($event, i)"></span>
        </div>
      </div>
    </div>
  </th>
</template>


<script>
export default {
  props: ['data', 'onResized'],
  data: () => ({
    debounceResize: null,
    sort: {
    },
  }),
  computed: {
    preference: function() {
      return usePreference();
    },
    ds: function () {
      return useDataSource();
    }
  },
  methods: {
    toggleSort: function (v) {
      this.sort[v] = this.sort[v] == 'asc' ? 'desc' : 'asc'
      this.ds.setSort(this.sort);
      this.ds.load();
    },
    startResize(e, index) {
      this.currentTH = this.$refs.ths;//[index];
      this.startX = e.clientX;
      this.startWidth = this.currentTH.offsetWidth;

      document.addEventListener("mousemove", this.onMouseMove);
      document.addEventListener("mouseup", this.stopResize);
    },
    onMouseMove(e) {
      if (!this.currentTH) return;
      const diffX = e.clientX - this.startX;
      const newWidth = this.startWidth + diffX;
      this.currentTH.style.width = Math.max(50, newWidth) + "px";

      var _this = this;

      if(this.debounceResize) {
        clearTimeout(_this.debounceResize)
      }

      this.debounceResize = setTimeout(() => {
        this.onResized(this.data, Math.max(50, newWidth))
        clearTimeout(_this.debounceResize)
      }, 500)
    },
    stopResize() {
      document.removeEventListener("mousemove", this.onMouseMove);
      document.removeEventListener("mouseup", this.stopResize);
      this.currentTH = null;
    },
  }
}
</script>

<style lang="scss" scoped>

td {
  button {
    margin: -2px 2px;
    // margin-top: -2px;
    // margin-bottom: -2px;
    padding-left: 5px;
    padding-right: 5px;
  }
}
th {
  position: relative;
  user-select: none;
}

.resizer {
  position: absolute;
  right: 0;
  top: 0;
  width: 5px;
  height: 100%;
  cursor: col-resize;
}

.resizable-table {
  border-collapse: collapse;
  min-width: max-content;
  table-layout: fixed; /* penting agar ellipsis bekerja */
  width: 100%;
  /* Ellipsis effect */
  .ellipsis-cell {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    min-width: 150px;
  }

  .cell-content {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}
</style>