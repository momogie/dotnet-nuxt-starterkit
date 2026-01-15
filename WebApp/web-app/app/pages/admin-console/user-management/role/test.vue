<script setup lang="ts">
import {
  Bell,
  Check,
  Globe,
  Home,
  Keyboard,
  Link,
  Lock,
  Menu,
  MessageCircle,
  Paintbrush,
  Settings,
  Video,
  SearchIcon,
} from "lucide-vue-next"

import { ref } from "vue"
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/shadcn/components/ui/breadcrumb"
import { Button } from "@/components/shadcn/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogTitle,
  DialogTrigger,
} from "@/components/shadcn/components/ui/dialog"
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
} from "@/components/shadcn/components/ui/sidebar"

const data = {
  nav: [
    { name: "Notifications", icon: Bell },
    { name: "Navigation", icon: Menu },
    // { name: "Home", icon: Home },
    // { name: "Appearance", icon: Paintbrush },
    // { name: "Messages & media", icon: MessageCircle },
    // { name: "Language & region", icon: Globe },
    // { name: "Accessibility", icon: Keyboard },
    // { name: "Mark as read", icon: Check },
    // { name: "Audio & video", icon: Video },
    // { name: "Connected accounts", icon: Link },
    // { name: "Privacy & visibility", icon: Lock },
    // { name: "Advanced", icon: Settings },
  ],
}

const open = ref(true)
</script>

<template>
  <div class="flex h-[300px]">
    <Sidebar collapsible="none" class="hidden md:flex rounded border">
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem v-for="item in data.nav" :key="item.name">
                <SidebarMenuButton
                  as-child
                  :is-active="item.name === 'Messages & media'"
                >
                  <a href="#">
                    <component :is="item.icon" />
                    <span>{{ item.name }}</span>
                  </a>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
    <main class="flex h-[445px] flex-1 flex-col overflow-hidden">
      <header class="flex h-5 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
        <div class="flex items-center gap-2 px-4">
          <Breadcrumb>
            <BreadcrumbList>
              <BreadcrumbItem class="hidden md:block">
                <BreadcrumbLink href="#">
                  Role Permissions
                </BreadcrumbLink>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </div>
      </header>
      <div class="px-4 py-2 border-b">
        <InputGroup>
          <InputGroupInput placeholder="Search..." />
          <InputGroupAddon>
            <SearchIcon />
          </InputGroupAddon>
        </InputGroup>
      </div>
      <div class="flex flex-1 flex-col gap-4 overflow-y-auto p-4 pt-0">
        <div
          v-for="i in 10"
          :key="i"
          class="aspect-video max-w-3xl rounded-xl bg-muted/50"
        />
      </div>
    </main>
  </div>
</template>
