import { useQuery } from "@tanstack/react-query";
import { appAxios } from "@/lib/app-axios";
import type { SocialInfo } from "@/models/domain/social-info";

interface SocialInfoListResponse {
  items: SocialInfo[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export const useRecentSocialInfo = (count: number = 10) => {
  return useQuery({
    queryKey: ["social-info", "recent", count],
    queryFn: async () => {
      // Use absolute-looking relative path and ensure it's /social-info
      // to match controller route [Route("api/[controller]")]
      const response = await appAxios.get<SocialInfoListResponse>(
        "/social-info",
        {
          params: {
            pageSize: count,
            page: 1
          }
        }
      );
      return response.data.items;
    },
    staleTime: 1000 * 60 // 1 minute
  });
};
