<template>
  <div>
    <div v-if="label" class="font-semibold text-xs mb-1">{{ label }}</div>
    <div
      class="border-accent rounded flex cursor-pointer justify-center border-2 border-dashed p-5"
      @drop="onFileDrop" @dragover="e => e.preventDefault()" @click="onAdd"
      v-if="multiple !== undefined || list.length == 0"
    >
      <div class="text-center flex">
        <span
          class="bg-base-200/80 text-base-content inline-flex size-14 items-center justify-center rounded-full me-5">
          <Icon name="lucide:cloud-upload"  size="40"/>
        </span>
        <div>
          <div class="mt-2 flex flex-wrap justify-center">
            <span class="text-base-content pe-1 text-base font-medium">Drop your file here or</span>
            <span class="link link-animated link-primary font-semibold">browse</span>
          </div>
          <p class="text-base-content/50 mt-1 text-xs">Pick a file up to 2MB.</p>
        </div>
      </div>
      <input ref="c-file-picker" 
        type="file" style="display: none" 
        @input="pick"
        :accept="accept"
      />
    </div>
    <div class="mt-4 space-y-2 empty:mt-0" 
      v-for="(item, i) in files" :key="i"
    >
      <div class="rounded border shadow-lg bg-base-100 p-4 dz-processing dz-error dz-complete complete">
        <div class="mb-1 flex items-center justify-between">
          <div class="flex items-center gap-x-3"> <span
              class="text-base-content/80 border-base-content/20 flex size-8 items-center justify-center rounded-lg border p-0.5"
              data-file-upload-file-icon=""> <img class="rounded-md hidden" data-dz-thumbnail=""> <svg
                xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"
                stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                class="shrink-0 size-5">
                <path d="M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z"></path>
                <path d="M14 2v4a2 2 0 0 0 2 2h4"></path>
              </svg></span>
            <div>
              <p class="text-base-content text-sm font-medium"> <span class="inline-block truncate align-bottom"
                  data-file-upload-file-name="">{{item.file.name}}</span><span
                  data-file-upload-file-ext=""></span> </p>
              <p class="text-base-content/50 text-xs" data-file-upload-file-size="">
                {{ $func.bytesToSize(item.file.size) }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-x-2"> 
            <button type="button" class="btn btn-sm btn-circle btn-text"
              data-file-upload-remove="" @click="() => onRemove(i)"> 
              <span class="icon-[tabler--trash] size-4 shrink-0"></span> 
            </button> 
          </div>
        </div>
        <div class="flex items-center gap-x-3 whitespace-nowrap">
          <!-- <div class="progress h-2" role="progressbar" :aria-valuenow="item.progress" aria-valuemin="0" aria-valuemax="100"
            data-file-upload-progress-bar="">
            <div class="progress-bar progress-primary file-upload-complete:progress-success transition-all duration-500"
              :style="`width: ${item.progress}%;`" data-file-upload-progress-bar-pane=""></div>
          </div>  -->
          
          <Progress class="w-full" :model-value="item.progress" />
          <span class="text-base-content mb-0.5 text-sm"> <span
              data-file-upload-progress-bar-value="">{{item.progress}}</span>% </span>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  props: ['multiple', 'modelValue', 'accept', 'label'],
  emits: ['update:modelValue'],
  data: () => ({
    files: [],
    data: [],
    extensions:[
      '7z', '7zip', 'ai', 'bin', 'bmp', 'cal', 'cs', 'css', 'csv',
      'dart', 'deb', 'doc', 'docs', 'docx', 'exe', 'gif', 'gz', 'gzip',
      'html', 'ico', 'jar', 'java', 'jpeg', 'jpg', 'js', 'json', 'jsx', 'kt', 'lib',
      'mp3', 'mp4', 'mpeg', 'ods', 'odt', 'pak', 'pdf', 'php', 'png', 'ppt', 'pptx',
      'rar', 'sass', 'scss', 'sql', 'svg', 'tar', 'tif', 'ts', 'txt',
      'wav', 'webp', 'woff', 'xls', 'xlsx', 'xml', 'zip'
    ],
    list: [],
  }),
  mounted: function () { },
  computed: {},
  watch: {
    modelValue: function(after) {
      if(!after) {
        this.data = this.list = this.files = [];
      }
      if(Array.isArray(after)) {
        if(after.length == 0)
          this.data = this.list = this.files = [];
      }
    }
  },
  methods: {
    pick: function (e) {
      if (e?.target?.files.length > 0) this.addFile(e?.target?.files[0]);
    },
    onAdd: function () {
      this.$refs["c-file-picker"]?.click();
    },
    addFile: function (file) {
      if(this.files.length > 10) return;
      var data = {
        key: file.name + (new Date()).getTime(),
        progress: 0,
        file: file
      };
      this.files.unshift(data);
      this.uploadImage(data);
      this.$refs["c-file-picker"].value = null;
    },
    onFileDrop: function (e) {
      e?.preventDefault();
      e.stopPropagation();
      // if (e.dataTransfer.files) 
      //   e.dataTransfer.files?.forEach(this.addFile);
    },
    onRemove: function (idx) {
      this.list.splice(idx, 1)
      this.files.splice(idx, 1)
      this.$emit('update:modelValue', this.list)
    },
    returnFile: function () {
      let arr = [];

      [...this.files, ...this.data].forEach(el => {
        if (el.AttachmentId || el.file) arr.push(el);
      });

      return arr;
    },
    getThumbnail: function(file) {
      return URL.createObjectURL(file);
    },
    getExtension: function(fileName) {
      if(!fileName) return;
      var d = fileName?.split('.');
      return d[d.length -1]
    },
    uploadImage: function(file) {
      this.isLoading = true;
      var data = new FormData();
      data.append('DocumentId', '')
      data.append('DocumentType', '')
      data.append('File', file.file)
      this.$http.post(`/attachment/upload`, data, {
        onUploadProgress: (progressEvent) => {
          console.log(this.files, file, progressEvent)
          var index = this.files.findIndex(p => p.key == file.key);
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          this.files[index].progress = percentCompleted;
        },
      })
        .then(({data})=> {
          this.list.unshift(data.Data)
          if(!this.multiple) 
            this.$emit('update:modelValue', this.list[0]);

          if(this.multiple)
            this.$emit('update:modelValue', this.list);
        })
        .finally(_ => this.isLoading = false)
    },
  },
};
</script>