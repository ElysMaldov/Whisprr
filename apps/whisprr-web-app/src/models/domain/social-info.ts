export interface SocialInfo {
  id: string;
  topicId?: string;
  topicName?: string;
  taskId?: string;
  platform: string;
  sourceId: string;
  sourceUrl?: string;
  content: string;
  author?: string;
  postedAt?: string;
  collectedAt: string;
  sentimentScore?: number;
}
