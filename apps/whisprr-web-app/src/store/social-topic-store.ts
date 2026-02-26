import { SocialTopic } from "@/models/domain/social-topic";
import { create } from "zustand";

interface SocialTopicState {
  topics: SocialTopic[];
  setTopics: (topics: SocialTopic[]) => void;
}

export const useSocialTopicStore = create<SocialTopicState>((set) => ({
  topics: [],
  setTopics: (topics) => set({ topics }),
}));
