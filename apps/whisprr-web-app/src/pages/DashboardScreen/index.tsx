import { SidebarInset, SidebarProvider } from "@/components/ui/sidebar";
import { useSocialTopicHub } from "@/hooks/useSocialTopicHub";
import { useRecentSocialInfo } from "@/hooks/useSocialInfo";
import { SiteHeader } from "@/pages/DashboardScreen/SiteHeader";
import { useEffect, useState } from "react";
import { AppSidebar } from "./AppSidebar";
import TopicCards from "./TopicCards";
import { PlatformType } from "@/models/domain/platform-type";
import type { SocialInfo } from "@/models/domain/social-info";
import { SocialInfoCard } from "@/components/SocialInfoCard";
import { MessageSquareQuote, Loader2 } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";

const SocialInfoSkeleton = () => (
  <div className="grid grid-cols-1 @[40rem]:grid-cols-2 @[60rem]:grid-cols-3 @[90rem]:grid-cols-4 gap-4">
    {[...Array(4)].map((_, i) => (
      <div key={i} className="h-[200px] rounded-xl border border-gray-100 p-4 space-y-3">
        <div className="flex items-center gap-2">
          <Skeleton className="h-4 w-4 rounded-full" />
          <Skeleton className="h-4 w-24" />
        </div>
        <Skeleton className="h-20 w-full" />
        <div className="flex justify-between">
          <Skeleton className="h-4 w-16" />
          <Skeleton className="h-4 w-12" />
        </div>
      </div>
    ))}
  </div>
);

const DashboardScreen = () => {
  // Setup WebSocket connection to Whisprr.Api
  const { isConnected, onNewInfo } = useSocialTopicHub();
  const [liveInfo, setLiveInfo] = useState<SocialInfo[]>([]);
  
  // Fetch initial data
  const { data: initialInfo, isLoading } = useRecentSocialInfo(10);

  useEffect(() => {
    if (!isConnected) return;

    // Subscribe to real-time events
    const unsubscribeNewInfo = onNewInfo((info) => {
      console.log("New social info received:", info);
      setLiveInfo((prev) => [info, ...prev].slice(0, 10));
    });

    // Cleanup subscriptions on unmount
    return () => {
      unsubscribeNewInfo();
    };
  }, [isConnected, onNewInfo]);

  // Merge initial and live data, avoiding duplicates if any
  const displayInfo = [...liveInfo];
  if (initialInfo) {
    initialInfo.forEach(info => {
      if (!displayInfo.some(di => di.id === info.id)) {
        displayInfo.push(info);
      }
    });
  }
  
  // Sort by collectedAt descending and limit to 10
  const finalInfo = displayInfo
    .sort((a, b) => new Date(b.collectedAt).getTime() - new Date(a.collectedAt).getTime())
    .slice(0, 10);

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
        <div className="flex flex-1 flex-col overflow-hidden">
          <div className="@container/main flex flex-1 flex-col gap-2 overflow-y-auto">
            <div className="flex flex-col gap-8 p-4 md:p-6">
              <div>
                <h2 className="text-2xl font-bold tracking-tight mb-4">Your Topics</h2>
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

              <div>
                <div className="flex items-center gap-2 mb-4">
                  <MessageSquareQuote className="h-5 w-5 text-primary" />
                  <h2 className="text-2xl font-bold tracking-tight">Live Feed</h2>
                  {isConnected ? (
                    <span className="flex h-2 w-2 rounded-full bg-green-500 animate-pulse" title="Connected" />
                  ) : (
                    <Loader2 className="h-3 w-3 text-muted-foreground animate-spin" />
                  )}
                </div>

                {isLoading ? (
                  <SocialInfoSkeleton />
                ) : finalInfo.length === 0 ? (
                  <div className="flex flex-col items-center justify-center py-12 px-4 border-2 border-dashed rounded-xl border-gray-200 bg-gray-50/50 text-center">
                    <p className="text-muted-foreground mb-1">Waiting for new social information...</p>
                    <p className="text-xs text-muted-foreground/60 italic">Real-time updates will appear here as they are collected.</p>
                  </div>
                ) : (
                  <div className="grid grid-cols-1 @[40rem]:grid-cols-2 @[60rem]:grid-cols-3 @[90rem]:grid-cols-4 gap-4">
                    {finalInfo.map((info) => (
                      <SocialInfoCard key={info.id} info={info} />
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
};

export default DashboardScreen;
