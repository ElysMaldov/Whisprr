import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { useSocialTopicHub } from "@/hooks/useSocialTopicHub";
import { SiteHeader } from "@/pages/DashboardScreen/SiteHeader";
import { useEffect } from "react";
import { AppSidebar } from "./AppSidebar";
import TopicCards from "./TopicCards";
import { PlatformType } from "@/models/domain/platform-type";

const DashboardScreen = () => {
  // Setup WebSocket connection to Whisprr.Api
  const { isConnected, onNewInfo } = useSocialTopicHub();

  useEffect(() => {
    if (!isConnected) return;

    // Subscribe to real-time events
    const unsubscribeNewInfo = onNewInfo((info) => {
      console.log("New social info received:", info);
      // TODO: Handle new social info (e.g., update UI, show notification)
    });

    // Cleanup subscriptions on unmount
    return () => {
      unsubscribeNewInfo();
    };
  }, [isConnected, onNewInfo]);

  return (
    <SidebarProvider
      style={
        {
          "--sidebar-width": "calc(var(--spacing) * 72)",
          "--header-height": "calc(var(--spacing) * 12)"
        } as React.CSSProperties
      }
    >
      <AppSidebar variant="inset" />
      <SidebarInset>
        <SiteHeader />
        <div className="flex flex-1 flex-col">
          <div className="@container/main flex flex-1 flex-col gap-2">
            <div className="flex flex-col gap-4 p-4">
              <TopicCards
                topics={[
                  {
                    id: "fe",
                    keywords: ["Meow"],
                    language: "en",
                    platform: PlatformType.Bluesky
                  }
                ]}
              />
            </div>
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
};

export default DashboardScreen;
