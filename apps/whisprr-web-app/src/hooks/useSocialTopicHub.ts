import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { SocialInfo } from "@/models/domain/social-info";

const HUB_URL = import.meta.env.VITE_API_URL
  ? `${import.meta.env.VITE_API_URL.replace("/api", "")}/hubs/social`
  : "http://localhost:5171/hubs/social";

export interface SocialTopicHubState {
  isConnected: boolean;
  error: Error | null;
}

export const useSocialTopicHub = () => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const token = localStorage.getItem("token");

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => token || "",
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connectionRef.current = connection;

    // Handle connection events
    connection.onreconnecting((err) => {
      console.log("SignalR reconnecting...", err);
      setIsConnected(false);
    });

    connection.onreconnected(() => {
      console.log("SignalR reconnected");
      setIsConnected(true);
      setError(null);
    });

    connection.onclose((err) => {
      console.log("SignalR connection closed", err);
      setIsConnected(false);
      if (err) {
        setError(err);
      }
    });

    // Start connection
    connection
      .start()
      .then(() => {
        console.log("SignalR connected");
        setIsConnected(true);
        setError(null);
      })
      .catch((err) => {
        console.error("SignalR connection error:", err);
        setError(err);
        setIsConnected(false);
      });

    // Cleanup on unmount
    return () => {
      connection
        .stop()
        .then(() => console.log("SignalR disconnected"))
        .catch((err) => console.error("SignalR disconnect error:", err));
    };
  }, []);

  // Subscribe to new info events
  const onNewInfo = (callback: (info: SocialInfo) => void) => {
    if (connectionRef.current) {
      connectionRef.current.on("OnNewInfo", callback);
    }
    return () => {
      connectionRef.current?.off("OnNewInfo", callback);
    };
  };

  // Subscribe to task status changed events
  const onTaskStatusChanged = (callback: (task: unknown) => void) => {
    if (connectionRef.current) {
      connectionRef.current.on("OnTaskStatusChanged", callback);
    }
    return () => {
      connectionRef.current?.off("OnTaskStatusChanged", callback);
    };
  };

  // Subscribe to new task events
  const onNewTask = (callback: (task: unknown) => void) => {
    if (connectionRef.current) {
      connectionRef.current.on("OnNewTask", callback);
    }
    return () => {
      connectionRef.current?.off("OnNewTask", callback);
    };
  };

  // Join a topic group to receive updates
  const joinTopic = async (topicId: string) => {
    if (connectionRef.current && isConnected) {
      await connectionRef.current.invoke("JoinTopic", topicId);
    }
  };

  // Leave a topic group
  const leaveTopic = async (topicId: string) => {
    if (connectionRef.current && isConnected) {
      await connectionRef.current.invoke("LeaveTopic", topicId);
    }
  };

  return {
    isConnected,
    error,
    onNewInfo,
    onTaskStatusChanged,
    onNewTask,
    joinTopic,
    leaveTopic
  };
};
