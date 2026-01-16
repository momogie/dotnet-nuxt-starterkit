

<template>
  <div>
    <div class="ms-4 border-b pb-2">
      <span class="font-semibold">Permissions</span>
      <!-- <pre>{{ selectedFeatures }}</pre> -->
       <!-- <pre>{{ selectedModule }}</pre> -->
    </div>
    <div class="flex h-[300px]">
      <div class="px-4 border-r h-[320px] pt-2">
        <div v-for="(item, i) in list">
          <Button size="sm" 
            :variant="selectedModuleIdx == i ? '' : 'outline'" 
            @click.prevent
          >
            <span>{{ item.Name }}</span>
          </Button>
        </div>
      </div>
      <main class="flex h-[320px] flex-1 flex-col overflow-hidden">
        <div class="flex border-b py-2">
          <div class="ms-4 pt-1">
            <Checkbox 
              :model-value="checkedAllItem(selectedModule?.Id)"
              @update:model-value="(v) => checkAll(v, selectedModule?.Id)"
            />
          </div>
          <div class="ms-4 flex-1 ms-2">
            <InputGroup class="text-sm h-8">
              <InputGroupInput  placeholder="Search..." />
              <InputGroupAddon  class="text-xs">
                <SearchIcon />
              </InputGroupAddon>
            </InputGroup>
          </div>
        </div>
        <div class="flex flex-1 flex-col gap-4 overflow-y-auto p-4 pt-0">
          <div v-if="selectedModule.Id">
            <div v-for="(item,i) in list[selectedModuleIdx].Features">
              <div class="flex border-b py-1">
                <div>
                  <Checkbox 
                    :model-value="checkedItem(item)"
                    @update:model-value="(v) => onChecked(v, item)"
                  />
                </div>
                <div class="ms-4">
                  <div class="text-sm">{{ item.Name }}</div>
                  <div class="text-xs text-gray-600">{{ item.Description }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script>
import { Button } from "@/components/shadcn/components/ui/button"
import{SearchIcon} from 'lucide-vue-next'
export default {
  props: ['modelValue'],
  emits: ['update:modelValue'],
  components: { Button, SearchIcon },
  data: () => ({
    isLoading: false,
    isLoaded: false,
    list: [],
    selectedModule: {},
    selectedModuleIdx: -1,
    selectedFeatures: [],
  }),
  mounted: function() {
    this.load();
  },
  methods: {
    load: function() {
      this.isLoaded = false;
      this.isLoaded = true;
      this.selectedModule = {}
      this.selectedModuleIdx = -1;
      this.$http.get('/identity/role/permissions')
        .then(({data}) => {
          this.list = data.Data.map(p => ({...p, Checked: false}));
          if(this.list.length > 0) {
            this.selectedModule = this.list[0]
            this.selectedModuleIdx = 0;
          }
          var allFeatures = this.list.flatMap(p => p.Features);
          this.selectedFeatures = allFeatures.filter(p => this.modelValue.some(c => c.Value == (p.ModuleId + '.' + p.Id)));
        })
        .finally(_ => this.isLoading = false)
    },
    checkedAllItem: function(f) {
      return this.selectedFeatures.filter(p => p.ModuleId == f).length == (this.selectedModule?.Features || []).length
        && (this.selectedModule?.Features || []).length > 0
      ;
    },
    checkedItem: function(v) {
      return this.selectedFeatures.some(p => (p.ModuleId + '.' + p.Id) == (v.ModuleId + '.' + v.Id));
    },
    checkAll: function(e, f) {
      if(!e) {
        this.selectedFeatures = this.selectedFeatures.filter(p => p.ModuleId != f);
        this.emmitValue();
        return;
      }
      this.selectedFeatures = this.selectedFeatures.filter(p => p.ModuleId != f);
      var all = this.list.find(p => p.Id == f).Features;
      this.selectedFeatures = [...this.selectedFeatures,...all];
      this.emmitValue();
    },
    onChecked: function(e, v) {
      if(!e) {
        var idx = this.selectedFeatures.findIndex(p => (p.ModuleId + '.' + p.Id) == (v.ModuleId + '.' + v.Id));
        if(idx >= 0) {
          this.selectedFeatures.splice(idx, 1)
        }
        this.emmitValue();
        return;
      }
      v.Checked = true;
      this.selectedFeatures.push(v)
      this.emmitValue();
    },
    emmitValue: function() {
      var list = this.selectedFeatures.map(p => ({ Type: 'FEATURE', Value: p.ModuleId + '.' + p.Id}));
      this.$emit('update:modelValue', list)
    }
  }
}
</script>