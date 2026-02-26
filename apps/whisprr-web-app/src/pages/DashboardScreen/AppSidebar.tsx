import * as React from "react";

import Logo from "@/components/Logo";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader
} from "@/components/ui/sidebar";
import { Camera, Computer, Settings } from "lucide-react";
import { NavMain } from "./NavMain";
import { NavUser } from "./NavUser";
import { useAuthStore } from "@/store/auth-store";
import { NavTopics } from "./NavTopics";

const data = {
  navMain: [
    {
      title: "Dashboard",
      url: "#",
      icon: Computer
    }
  ],
  navClouds: [
    {
      title: "Capture",
      icon: Camera,
      isActive: true,
      url: "#",
      items: [
        {
          title: "Active Proposals",
          url: "#"
        },
        {
          title: "Archived",
          url: "#"
        }
      ]
    }
  ],
  navSecondary: [
    {
      title: "Settings",
      url: "#",
      icon: Settings
    }
  ]
};

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { user } = useAuthStore();

  return (
    <Sidebar
      collapsible="offcanvas"
      {...props}
    >
      <SidebarHeader>
        <Logo />
        <p className="text-base font-semibold">Whisprr</p>
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={data.navMain} />
        <NavTopics
          items={[
            {
              name: "Topic A",
              url: "fefe"
            }
          ]}
        />
      </SidebarContent>
      <SidebarFooter>
        {user && (
          <NavUser
            user={{
              avatar: "",
              email: user?.email,
              name: user?.displayName || ""
            }}
          />
        )}
      </SidebarFooter>
    </Sidebar>
  );
}
