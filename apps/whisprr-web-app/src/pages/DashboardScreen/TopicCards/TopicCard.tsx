import { Badge } from "@/components/ui/badge";
import {
  Card,
  CardDescription,
  CardHeader,
  CardTitle
} from "@/components/ui/card";
import { SocialTopic } from "@/models/domain/social-topic";

export interface TopicCardProps {
  topic: SocialTopic;
  onClick?: () => void;
}

const TopicCard = ({ topic, onClick }: TopicCardProps) => {
  return (
    <Card onClick={onClick} className={onClick ? "cursor-pointer hover:shadow-md transition-shadow" : undefined}>
      <CardHeader>
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg">{topic.keywords.join(" ")}</CardTitle>
          <Badge variant="secondary">{topic.platform}</Badge>
        </div>
        {topic.description && (
          <CardDescription>{topic.description}</CardDescription>
        )}
      </CardHeader>
    </Card>
  );
};

export default TopicCard;
