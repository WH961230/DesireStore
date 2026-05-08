# -*- coding: utf-8 -*-
"""
生成10个游戏图标 PNG资源
风格：深蓝色背景 + 白色线条 + 红色强调
"""

from PIL import Image, ImageDraw
import os

# 颜色定义
BG_COLOR = (26, 30, 46)       # #1a1e2e 深蓝背景
LINE_COLOR = (255, 255, 255) # 白色线条
ACCENT_COLOR = (233, 77, 90)  # #e94d5a 红色强调

ICON_SIZE = 128
OUTPUT_DIR = "Assets/Resources/Icons"

# 确保输出目录存在
os.makedirs(OUTPUT_DIR, exist_ok=True)

def create_icon(name, draw_func):
    """创建单个图标"""
    img = Image.new('RGBA', (ICON_SIZE, ICON_SIZE), BG_COLOR + (255,))
    draw = ImageDraw.Draw(img)
    draw_func(draw)
    filepath = os.path.join(OUTPUT_DIR, f"{name}.png")
    img.save(filepath)
    print(f"Created: {filepath}")
    return filepath

def draw_diamond(draw):
    """菱形"""
    cx, cy = ICON_SIZE // 2, ICON_SIZE // 2
    size = 40
    points = [
        (cx, cy - size),      # 上
        (cx + size, cy),      # 右
        (cx, cy + size),      # 下
        (cx - size, cy),      # 左
    ]
    draw.polygon(points, outline=LINE_COLOR, width=3)

def draw_star(draw):
    """星星 - 5角星"""
    cx, cy = ICON_SIZE // 2, ICON_SIZE // 2
    outer_r = 40
    inner_r = 18
    points = []
    for i in range(10):
        r = outer_r if i % 2 == 0 else inner_r
        angle = -90 + i * 36
        rad = angle * 3.14159 / 180
        x = cx + r * (1 if abs(rad) > 3.14 else 1) * (1 if abs(angle) < 90 or abs(angle) > 270 else -1 if abs(angle) > 90 and abs(angle) < 270 else 1)
        # 简化计算
        angle_rad = (i * 36 - 90) * 3.14159 / 180
        x = cx + r * (1 if i % 2 == 0 else 0.4) * (1 if (i // 2) % 2 == 0 else -1 if (i // 2) in [1, 4] else 1)
        y = cy + r * (1 if i % 2 == 0 else 0.4) * (1 if i in [0, 1, 2, 3] else -1 if i in [5, 6, 7, 8] else 1)
    # 正确的5角星绘制
    cx, cy = 64, 64
    outer_r = 42
    inner_r = 18
    points = []
    for i in range(10):
        r = outer_r if i % 2 == 0 else inner_r
        angle = (i * 36 - 90) * 3.14159 / 180
        x = cx + r * (1 if abs(angle) < 3.14 else -1) * abs((1 if abs(angle) < 3.14 else -1) * 1)
        # 简化
        angle_deg = i * 36 - 90
        x = cx + r * (1 if 0 <= (angle_deg + 360) % 360 < 180 else -1) * abs(1)
        y = cy + r * (1 if 90 <= (angle_deg + 360) % 360 < 270 else -1)
    # 正确方法
    angles = []
    for i in range(10):
        angle_deg = i * 36 - 90
        angles.append(angle_deg)
    points = []
    for i, angle_deg in enumerate(angles):
        r = outer_r if i % 2 == 0 else inner_r
        rad = angle_deg * 3.14159 / 180
        points.append((cx + r * (1 if 0 <= ((angle_deg + 360) % 360) < 180 else -1) * abs(1), 0))
    # 完全重写
    cx, cy = 64, 64
    outer_r = 42
    inner_r = 18
    coords = []
    for i in range(10):
        angle = (i * 36 - 90) * 3.14159 / 180
        r = outer_r if i % 2 == 0 else inner_r
        x = cx + r * (1 if 0 <= ((i * 36) % 360) < 180 else -1) * abs(1)
        y = cy + r * (1 if 90 <= ((i * 36) % 360) < 270 else -1)
    # 真正简化
    cx, cy = 64, 64
    outer = 42
    inner = 18
    star_points = []
    for i in range(10):
        r = outer if i % 2 == 0 else inner
        angle = (i * 36 - 90) * 3.14159 / 180
        x = cx + r * (1 if 0 <= (i * 36) % 360 < 180 else -1) * abs(1)
        y = cy + r * (1 if 90 <= (i * 36) % 360 < 270 else -1)
        star_points.append((x, y))
    draw.polygon(star_points, outline=LINE_COLOR, width=3)

def draw_gift(draw):
    """礼盒"""
    # 盒子
    draw.rectangle([30, 50, 98, 100], outline=LINE_COLOR, width=3)
    # 蝴蝶结
    draw.ellipse([54, 35, 74, 55], outline=ACCENT_COLOR, width=3)
    draw.ellipse([54, 35, 74, 55], outline=ACCENT_COLOR, width=3)
    draw.line([64, 40, 64, 55], fill=ACCENT_COLOR, width=2)
    draw.line([30, 70, 98, 70], fill=ACCENT_COLOR, width=2)
    draw.line([64, 50, 64, 100], fill=ACCENT_COLOR, width=2)

def draw_sword(draw):
    """剑"""
    # 剑身
    draw.polygon([(64, 15), (70, 80), (64, 75), (58, 80)], outline=LINE_COLOR, width=3)
    # 剑柄
    draw.line([50, 80, 78, 80], fill=LINE_COLOR, width=3)
    # 剑柄装饰
    draw.line([58, 85, 70, 85], fill=ACCENT_COLOR, width=2)
    draw.polygon([(60, 85), (64, 105), (68, 85)], outline=LINE_COLOR, width=3)

def draw_shield(draw):
    """盾牌"""
    # 盾牌主体
    draw.polygon([(64, 20), (95, 35), (95, 65), (64, 105), (33, 65), (33, 35)], outline=LINE_COLOR, width=3)
    # 盾牌装饰
    draw.line([64, 35, 64, 90], fill=ACCENT_COLOR, width=2)
    draw.line([45, 55, 83, 55], fill=ACCENT_COLOR, width=2)

def draw_heart(draw):
    """心形"""
    # 使用两个圆和底部三角形组合
    draw.arc([30, 30, 65, 65], 0, 180, fill=ACCENT_COLOR, width=3)
    draw.arc([63, 30, 98, 65], 0, 180, fill=ACCENT_COLOR, width=3)
    draw.polygon([(30, 55), (64, 95), (98, 55)], outline=ACCENT_COLOR, width=3)
    # 填充连接处
    draw.line([64, 55, 64, 95], fill=ACCENT_COLOR, width=2)

def draw_lightning(draw):
    """闪电"""
    draw.polygon([(75, 15), (45, 65), (60, 65), (50, 110), (85, 55), (70, 55)], outline=ACCENT_COLOR, width=3)

def draw_crown(draw):
    """皇冠"""
    # 皇冠底座
    draw.polygon([(25, 60), (30, 35), (45, 50), (64, 25), (83, 50), (98, 35), (103, 60), (25, 60)], outline=ACCENT_COLOR, width=3)
    # 底边
    draw.line([25, 95, 103, 95], fill=ACCENT_COLOR, width=3)
    draw.line([25, 60, 25, 95], fill=ACCENT_COLOR, width=3)
    draw.line([103, 60, 103, 95], fill=ACCENT_COLOR, width=3)
    draw.line([25, 95, 103, 95], fill=ACCENT_COLOR, width=3)
    # 宝石装饰
    draw.ellipse([56, 70, 72, 86], fill=ACCENT_COLOR)

def draw_potion(draw):
    """药水瓶"""
    # 瓶身
    draw.polygon([(40, 45), (48, 45), (38, 90), (90, 90), (80, 45), (88, 45), (88, 35), (40, 35)], outline=LINE_COLOR, width=3)
    # 瓶口
    draw.rectangle([48, 25, 80, 35], outline=LINE_COLOR, width=3)
    # 药水
    draw.polygon([(42, 65), (86, 65), (90, 90), (38, 90)], fill=ACCENT_COLOR)
    # 气泡
    draw.ellipse([55, 72, 62, 79], fill=BG_COLOR + (255,))

def draw_question(draw):
    """问号/目标"""
    # 圆形目标
    draw.ellipse([20, 20, 108, 108], outline=LINE_COLOR, width=3)
    draw.ellipse([38, 38, 90, 90], outline=LINE_COLOR, width=2)
    draw.ellipse([56, 56, 72, 72], fill=ACCENT_COLOR)
    # 问号
    draw.arc([50, 45, 78, 85], 200, 160, fill=LINE_COLOR, width=4)
    draw.point((64, 92), fill=LINE_COLOR)
    draw.ellipse([60, 92, 68, 100], outline=LINE_COLOR, width=2)

# 创建所有图标
icons = [
    ("icon_diamond", draw_diamond),
    ("icon_star", draw_star),
    ("icon_gift", draw_gift),
    ("icon_sword", draw_sword),
    ("icon_shield", draw_shield),
    ("icon_heart", draw_heart),
    ("icon_lightning", draw_lightning),
    ("icon_crown", draw_crown),
    ("icon_potion", draw_potion),
    ("icon_question", draw_question),
]

print("开始生成图标...")
for name, draw_func in icons:
    create_icon(name, draw_func)

print("\n所有图标生成完成!")
print(f"输出目录: {os.path.abspath(OUTPUT_DIR)}")
