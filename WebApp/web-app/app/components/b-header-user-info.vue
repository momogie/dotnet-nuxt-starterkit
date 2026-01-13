<script setup lang="ts">
import {
  ChevronsUpDown,
  ChevronDown,
  BadgeCheck,
  LogOut,
  Bell,
  CreditCard,
} from "lucide-vue-next"

import { useSidebar } from '@/components/shadcn/components/ui/sidebar'
import { useUser } from "~/stores/user";
import axios from "axios";

const { isMobile } = useSidebar()

const signOut = function() {
  // const config = useRuntimeConfig()
  // const BASE_URL = process.dev ? config.public.baseUrl : '/api';
  location.href = '/auth/signout'
}

const navigateUrl = function(url: string) {
  location.href = url;
}

const user: any = useUser();
const { $modal } : any = useNuxtApp();
</script>

<template>
  <div>
    <DropdownMenu>
      <DropdownMenuTrigger as-child>
        <SidebarMenuButton
          size="lg"
          class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
        >
      
          <Avatar>
            <AvatarImage v-if="user.data?.ImageUrl" :src="user.data?.ImageUrl" alt="User" />
            <!-- <AvatarImage :src="user.data?.avatar" :alt="user.data?.Name" /> -->
            <AvatarFallback class="rounded-lg">
              {{user.data?.Name?.substring(0, 2).toUpperCase()}}
            </AvatarFallback>
          </Avatar>
          <!-- <Avatar class="h-8 w-8 rounded-lg">
            <AvatarFallback class="rounded-lg">
              {{user.data?.Name.substring(0, 2).toUpperCase()}}
            </AvatarFallback>
          </Avatar> -->
          <div class="grid flex-1 text-left text-sm leading-tight">
            <span class="truncate font-medium">{{ user.data?.Name }} </span>
            <span class="truncate text-xs">{{ user.data?.Email }}</span>
          </div>
          <ChevronDown class="ml-auto size-4" />
        </SidebarMenuButton>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        class="w-[--reka-dropdown-menu-trigger-width] min-w-56 rounded-lg"
      >
        <DropdownMenuLabel class="p-0 font-normal">
          <div class="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
            <Avatar class="h-8 w-8 rounded-lg">
              <!-- <AvatarImage :src="user.data?.avatar" :alt="user.data?.Name" /> -->
              <AvatarImage v-if="user.data?.ImageUrl" :src="user.data?.ImageUrl" alt="User" />
              <AvatarFallback class="rounded-lg">
                {{user.data?.Name?.substring(0, 2).toUpperCase()}}
              </AvatarFallback>
            </Avatar>
            <div class="grid flex-1 text-left text-sm leading-tight">
              <span class="truncate font-semibold">{{ user.data?.Name }}</span>
              <span class="truncate text-xs">{{ user.data?.Email }}</span>
            </div>
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          <DropdownMenuItem @click="() => navigateUrl('/profile')">
            <BadgeCheck />
            Account
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuGroup>
          <DropdownMenuItem @click="() => navigateUrl('/app')">
            <Icon name="ph:terminal-window" size="18" />
            Admin Console
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <!-- <DropdownMenuGroup>
          <DropdownMenuItem @click="$modal.show('change-password')">
            <Icon name="ph:lock" size="18" />
            Change Password
          </DropdownMenuItem>
        </DropdownMenuGroup> -->
        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          <DropdownMenuItem>
            <CreditCard />
            Billing
          </DropdownMenuItem>
          <DropdownMenuItem>
            <Bell />
            Notifications
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuSeparator />
        <DropdownMenuItem  @click="signOut">
          <LogOut/>
          Log out
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  </div>
</template>