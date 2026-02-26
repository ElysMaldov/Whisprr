import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { SocialInfo } from "@/models/domain/social-info";
import { ExternalLink, Twitter, MessageCircle, Globe } from "lucide-react";

interface SocialInfoCardProps {
  info: SocialInfo;
}

const PlatformIcon = ({ platform }: { platform: string }) => {
  const p = platform.toLowerCase();
  if (p.includes("twitter") || p.includes("x")) return <Twitter className="h-4 w-4" />;
  if (p.includes("bluesky")) return <Globe className="h-4 w-4 text-blue-500" />;
  if (p.includes("mastodon")) return <MessageCircle className="h-4 w-4 text-purple-600" />;
  return <Globe className="h-4 w-4 text-gray-400" />;
};

const formatRelativeTime = (date: Date) => {
  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
  
  if (diffInSeconds < 60) return "just now";
  
  const diffInMinutes = Math.floor(diffInSeconds / 60);
  if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
  
  const diffInHours = Math.floor(diffInMinutes / 60);
  if (diffInHours < 24) return `${diffInHours}h ago`;
  
  const diffInDays = Math.floor(diffInHours / 24);
  return `${diffInDays}d ago`;
};

export const SocialInfoCard = ({ info }: SocialInfoCardProps) => {
  const getSentimentColor = (score?: number) => {
    if (score === undefined || score === null) return "bg-gray-100 text-gray-800";
    if (score > 0.2) return "bg-green-100 text-green-800";
    if (score < -0.2) return "bg-red-100 text-red-800";
    return "bg-yellow-100 text-yellow-800";
  };

  const getSentimentLabel = (score?: number) => {
    if (score === undefined || score === null) return "Neutral";
    if (score > 0.2) return "Positive";
    if (score < -0.2) return "Negative";
    return "Neutral";
  };

  const date = info.postedAt ? new Date(info.postedAt) : new Date(info.collectedAt);

  return (
    <Card className="h-full overflow-hidden flex flex-col transition-all hover:shadow-md border-gray-200">
      <CardHeader className="p-4 pb-2 flex flex-row items-start justify-between space-y-0">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2">
            <PlatformIcon platform={info.platform} />
            <span className="font-semibold text-sm truncate max-w-[150px]">
              {info.author || "Unknown Author"}
            </span>
            <span className="text-xs text-muted-foreground">
              • {formatRelativeTime(date)}
            </span>
          </div>
          {info.topicName && (
            <Badge variant="outline" className="w-fit text-[10px] py-0 h-4">
              {info.topicName}
            </Badge>
          )}
        </div>
        {info.sourceUrl && (
          <a
            href={info.sourceUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="text-muted-foreground hover:text-primary transition-colors"
          >
            <ExternalLink className="h-4 w-4" />
          </a>
        )}
      </CardHeader>
      <CardContent className="p-4 pt-2 flex-grow overflow-hidden">
        <p className="text-sm text-foreground/90 line-clamp-4 leading-relaxed whitespace-pre-wrap">
          {info.content}
        </p>
      </CardContent>
      <CardFooter className="p-4 pt-0 flex items-center justify-between border-t border-gray-50 mt-auto bg-gray-50/30">
        <Badge className={`text-[10px] border-none shadow-none ${getSentimentColor(info.sentimentScore)}`}>
          {getSentimentLabel(info.sentimentScore)} {info.sentimentScore ? `(${info.sentimentScore.toFixed(2)})` : ""}
        </Badge>
        <span className="text-[10px] text-muted-foreground font-mono">
          ID: {info.sourceId.substring(0, 8)}...
        </span>
      </CardFooter>
    </Card>
  );
};
