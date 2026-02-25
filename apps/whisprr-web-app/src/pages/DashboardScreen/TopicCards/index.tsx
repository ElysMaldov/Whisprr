import { SocialTopic } from "@/models/domain/social-topic";
import TopicCard from "./TopicCard";

export interface TopicCardsProps {
  topics: SocialTopic[];
}

const TopicCards = ({ topics }: TopicCardsProps) => {
  if (topics.length === 0) {
    return (
      <div className="text-muted-foreground text-center py-8">
        No topics yet.
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {topics.map((topic) => (
        <TopicCard
          key={topic.id}
          topic={topic}
          onClick={() => {}}
        />
      ))}
    </div>
  );
};

export default TopicCards;
