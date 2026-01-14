<template>
  <div class="w-full">
    <div class="flex justify-between">
      <div class="flex">
        <Popover>
          <PopoverTrigger as-child>
            <Button variant="outline" size="icon-sm" class="mr-1">
              <Icon name="ph:faders-fill" size="16" />
            </Button>
          </PopoverTrigger>
          <PopoverContent class="w-60" :align="`start`">
            <div class="grid">
              <div class="space-y-2 border-b -mx-4 px-3 pb-3 flex">
                <!-- <Checkbox /> -->
                <h4 class="font-medium leading-none text-sm mt-1">
                  Field to Show
                </h4>
              </div>
              <!-- <DropdownMenuSeparator /> -->
              <div class="max-h-64 overflow-y-auto -mx-4 ps-3">
                <div v-for="(col, i) in (columnList || [])">
                  <Checkbox :id="`col_show_${i}`" v-model="col.visible" @update:model-value="(v) => {
                    col.visible = v;
                    updateColumnVisibility(v, col)
                  }">
                  </Checkbox>
                  <span class="text-xs ms-2">{{ col.label }}</span>
                </div>
              </div>
            </div>
          </PopoverContent>
        </Popover>
        <div class="grid w-80">
          <InputGroup class="text-sm h-8">
            <InputGroupInput placeholder="Enter search query" @keyup="search" />
            <InputGroupAddon align="inline-start">
              <DropdownMenu>
                <DropdownMenuTrigger as-child>
                  <InputGroupButton variant="ghost" class="text-xs">
                    <span v-if="selectedKeyword?.key">
                      {{ selectedKeyword.label }}
                    </span>
                    <span v-else>
                      Search In...
                    </span>
                    <!-- <Icon name="ph:chevron-down-bold" size="12"/> -->
                    <!-- <ChevronDownIcon class="size-3" /> -->
                  </InputGroupButton>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" class="[--radius:0.95rem]">
                  <DropdownMenuItem v-for="(item, i) in (columns || []).filter(p => p.searchable === true)"
                    @click="() => selectedKeyword = item">
                    {{ item.label }}
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </InputGroupAddon>
          </InputGroup>
        </div>
      </div>
      <div class="flex h-5">
        <template
          v-for="(item, i) in (buttons || []).filter(p => (p.showOnChecked == null || !p.showOnChecked || (p?.showOnChecked === true && checkedList.length > 0)))">
          <Separator orientation="vertical" class="ms-2 mt-1" v-if="item.type?.toLowerCase() == 'separator'" />
          <Button variant="outline" size="sm" @click="(e) => onTopButtonClick(e, checkedList, item.onClick, i)"
            :disabled="isLoadingButtons[`btn_top_${i}`] === true" class="ms-2" v-else>
            <Icon :name="item.icon" size="16" />
            {{ item.label }}
          </Button>
        </template>
        <Separator orientation="vertical" class="mx-2 mt-1" v-if="(buttons || []).length > 0" />
        <TooltipProvider v-if="(exportTemplates || []).length > 0">
          <Tooltip>
            <TooltipTrigger as-child>
              <Button variant="outline" size="icon-sm" class="mr-1" @click="$modal.show('export-data')">
                <Icon name="ph:download-bold" size="16" />
              </Button>
            </TooltipTrigger>
            <TooltipContent>
              <p>Export</p>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
        <!-- <TooltipProvider v-if="(importTemplates || []).length > 0">
          <Tooltip>
            <TooltipTrigger as-child>
              <Button variant="outline" size="icon-sm" class="mr-1" @click="$modal.show('import-data')">
                <Icon name="ph:upload-bold" size="16" />
              </Button>
            </TooltipTrigger>
            <TooltipContent>
              <p>Import</p>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider> -->
        <TooltipProvider>
          <Tooltip>
            <TooltipTrigger as-child>
              <Button variant="outline" size="icon-sm" @click="refresh">
                <Icon name="ph:arrows-clockwise-bold" size="16" />
              </Button>
            </TooltipTrigger>
            <TooltipContent>
              <p>Refresh</p>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
        <Separator orientation="vertical" class="mx-2 mt-1" v-if="configurationPages" />
        <b-grid-setup :list="configurationPages" v-if="configurationPages" />
        <Separator orientation="vertical" class="mx-2 mt-1" v-if="backable !== undefined && backable !== false" />
        <TooltipProvider v-if="backable !== undefined && backable !== false">
          <Tooltip>
            <TooltipTrigger as-child>
              <Button variant="outline" size="sm" @click="() => $router.back()">
                <Icon name="ph:arrow-bend-double-up-left-bold" size="16" />
                Go Back
              </Button>
            </TooltipTrigger>
            <TooltipContent>
              <p>Go Back</p>
            </TooltipContent>
          </Tooltip>
        </TooltipProvider>
      </div>
    </div>
    <div class="overflow-x-auto overflow-y-auto mt-5 w-full"
      :style="`height: calc(100vh - 192px); max-width: calc(100vw - ${sidebarState ? 300 : 80}px);`">
      <table ref="table"  class="w-full text-xs  border-gray-200 rounded-lg  resizable-table">
        <thead class=" text-gray-700 uppercase sticky top-0 z-1 bg-background">
          <tr class=" border-b-2">
            <th style="position: sticky;width: 25px; inset-inline-start: 0;z-index: 9;"  class="ps-0 pe-1 sticky left-0 z-1 w-2 bg-background">
              <Checkbox id="grid_check_all" class="m-0" @update:model-value="(v) => checkAll(v)"
                :model-value="checkedList.length > 0 && checkedList.length == (ds.data.data || []).length" />
            </th>
          
            <b-grid-header
              v-for="(item, i) in (columnList || []).filter(p => p.visible !== false)"
              :data="item"
              v-on:resized="columnResized"
            />
            <th class="px-2 py-1 text-left sticky right-0 w-2 bg-background" style="position: sticky;width: 35px; inset-inline-end: 0;"  >

            </th>
          </tr>
        </thead>
        <tbody
          class="divide-y [&>tr:nth-child(even)]:bg-gray-100 dark:[&>tr:nth-child(even)]:bg-gray-800 dark:text-gray-100">
          <tr>
            <td class="text-center" :colspan="`${columns?.length + 2}`" v-if="ds.isLoading">
              <div class="w-full h-60 justify-items-center justify-center justify-self-center p-10">
                <Button variant="secondary" size="sm">
                  <Spinner />
                  Loading...
                </Button>
              </div>
            </td>
          </tr>
          <tr v-for="(item, i) in (ds.data.Items || [])" v-if="!ds.isLoading">
            <td class="px-1 sticky left-0  bg-background">
              <Checkbox :id="`terms_1_${i}`"
                @update:model-value="(v) => check(v, (keys || []).length > 0 ? ((keys || []).map(p => item[p]).join(';')) : item.Id)"
                :model-value="checkedList.some(p => p == ((keys || []).length > 0 ? ((keys || []).map(p => item[p]).join(';')) : item.Id))" />
            </td>
            <td class="px-2 py-1 truncate max-w-[120px]"
              v-for="col in (columnList || []).filter(p => p.visible !== false)" :class="col.class">
              <span v-if="col.type == 'date'">
                {{ $func.formatDate(item[col.PropertyName]) }}
              </span>
              <span v-else-if="col.type == 'datetime'">
                {{ $func.formatDateTime(item[col.PropertyName]) }}
              </span>
              <span v-else-if="col.type == 'time'">
                {{ $func.formatTime(item[col.PropertyName]) }}
              </span>
              <span v-else-if="col.type == 'day'">
                {{ $func.formatDay(item[col.PropertyName]) }}
              </span>
              <span v-else-if="col.type == 'money'">
                {{ $func.formatMoney(item[col.PropertyName]) }}
              </span>
              <span v-else-if="col.PropertyType == 'Boolean'">
                <Checkbox :value="() => item[col.PropertyName]" :default-value="item[col.PropertyName]" class="pointer-events-none" />
                <!-- <Icon name="ph:check-square-bold" size="16" v-if="item[col.PropertyName] === true"
                  class="text-green-600"
                />
                <Icon name="ph:x-square-bold" size="16" v-else
                  class="text-red-600"
                /> -->
              </span>
              <span v-else-if="col.keyShow">
                {{ item[col.keyShow] === col.valShow ? item[col.PropertyName] : '' }}
              </span>
              <span v-else>
                  {{ item[col.PropertyName] }}
              </span>
            </td>
            <td class="px-2 py-1 pe-5 sticky right-0  bg-background p-0"  style="position: sticky;inset-inline-end: 0;" >
              <b-grid-item-action :data="item" :actions="actions" />
            </td>
          </tr>
        </tbody>
      </table>
      <Empty class="from-muted/50 to-background from-30%" v-if="!ds.isLoading && ds.data.Items.length == 0">
        <EmptyHeader>
          <EmptyMedia variant="icon">
            <Icon name="ph:note-blank" size="43" />
          </EmptyMedia>
          <EmptyTitle>No Data Available</EmptyTitle>
          <EmptyDescription>
            You're all caught up. New data will appear here.
          </EmptyDescription>
        </EmptyHeader>
        <EmptyContent>
          <Button variant="outline" size="sm" @click="refresh">
            <Icon name="ph:arrows-clockwise-bold" size="16" />
            Refresh
          </Button>
        </EmptyContent>
      </Empty>
    </div>
    <b-grid-pagination class="mt-2" />
    <!-- <shared-import-data />
    <shared-export-data :templates="exportTemplates" :filters="exportFilters" /> -->
  </div>
</template>

<script>

export default {
  props: [
    'source', 'columns', 'sortList', 'keys',
    'configurationPages', 'backable',
    'buttons', 'actions', 'defaultFilter', 'initialFilter',
    'importTemplates', 'exportTemplates', 'exportFilters'
  ],
  data: () => ({
    checkedList: [],
    isLoadingButtons: {},
    selectedKeyword: {},
    keyword: null,
    debounce: null,
    sort: {
    },
    columnList: [],
    startX: 0,
    startWidth: 0,
    debounceResize: null,
    currentTh: null
  }),
  setup() {
    const sidebarState = useCookie('sidebar_state')
    return { sidebarState }
  },
  computed: {
    user: function() {
      return useUser();
    },
    app: function () {
      return useApp();
    },
    ds: function () {
      return useDataSource();
    }
  },
  mounted: function () {
    this.ds.setDataSource(this.source)
    this.ds.setFilter(this.defaultFilter || [])
    // this.ds.setSort(this.sortList);
    this.ds.load().then((data) => {
      console.log(data)
      this.columnList = data.Data.Headers;
    });
    // this.columnList = this.columns.map(p => ({ ...p, visible: true }))
    // this.columnList.forEach((v, i) => {
    //   const colCached = JSON.parse(localStorage.getItem(`tb_col_${this.source}_${this.user.data.UserId}`) || '[]');
    //   if(colCached.some(p => p.key == v.key)) {
    //     this.columnList[i].visible = colCached.find(p => p.key == v.key)?.visible ?? true;
    //     if(!isNaN(colCached.find(p => p.key == v.key)?.width))
    //       this.columnList[i].width = colCached.find(p => p.key == v.key)?.width;
    //     // console.log(`tb_col_${this.source}_${this.user.data.UserId}`,colCached.find(p => p.key == v.key)?.visible,this.columnList[i])
    //   }
    //   else {
    //     this.columnList[i].visible = true;
    //   }
    // })
    // if ((this.columnList || []).filter(p => p.searchable === true).length > 0) {
    //   this.selectedKeyword = this.columnList?.filter(p => p.searchable === true)[0];
    // }
  },
  methods: {
    search: function (e) {
      if (this.debounce) {
        clearInterval(this.debounce)
      }
      this.debounce = setTimeout(() => {
        var filter = [
          ...(this.defaultFilter || []),
          [this.selectedKeyword.key, 'like', `%${e.target.value ?? ''}%`]
        ]
        this.ds.setModelName(this.source)
        this.ds.setFilter(filter)
        this.ds.setSort(this.sortList);
        this.ds.load();
      }, 300);
    },
    toggleSort: function (v) {
      this.sort.by = v;
      this.sort.direction = this.sort.direction == 'asc' ? 'desc' : 'asc';
      this.ds.setSort([`${v} ${this.sort.direction}`]);
      this.ds.load();
    },
    refresh: function () {
      // this.ds.setModelName(this.source)
      // this.ds.setSort(this.sortList);
      this.ds.load();
    },
    checkAll: function (check = false) {
      if (check)
        this.checkedList = (this.ds.data?.data || []).map(p => (this.keys || []).length > 0 ? ((this.keys || []).map(c => p[c]).join(';')) : p.Id)
      else
        this.checkedList = [];
    },
    check: function (check = false, id) {
      if (check)
        this.checkedList.push(id)
      else
        this.checkedList = (this.ds.data?.data || []).filter(p => this.checkedList.some(c => c == (this.keys || []).length > 0 ? ((this.keys || []).map(c => p[c]).join(';')) : p.Id)).filter(p => ((this.keys || []).length > 0 ? ((this.keys || []).map(c => p[c]).join(';')) : p.Id) != id).map(p => (this.keys || []).length > 0 ? ((this.keys || []).map(c => p[c]).join(';')) : p.Id)
    },
    updateColumnVisibility: function() {
      // console.log(this.columnList, this.user)
      localStorage.setItem(`tb_col_${this.source}_${this.user.data.UserId}`, JSON.stringify(this.columnList));
    },
    onTopButtonClick: function (e, checkedList, onClick, i) {
      this.isLoadingButtons[`btn_top_${i}`] = true;
      onClick(e, checkedList)?.finally(_ => {
        this.checkedList = [];
        this.isLoadingButtons[`btn_top_${i}`] = false;
      })
    },
    columnResized: function(a, b) {
      var colIdx = this.columnList.findIndex(p => p.key == a.key);
      this.columnList[colIdx].width = b;
      console.log(this.columnList[colIdx])
      localStorage.setItem(`tb_col_${this.source}_${this.user.data.UserId}`, JSON.stringify(this.columnList));
    }
  }
}
</script>

<style lang="scss" scoped>

// td {
//   button {
//     margin: -2px 2px;
//     // margin-top: -2px;
//     // margin-bottom: -2px;
//     padding-left: 5px;
//     padding-right: 5px;
//   }
// }

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